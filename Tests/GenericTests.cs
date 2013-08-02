using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using LubyTransform.Transform;

namespace Tests
{
	public static class GenericTests
	{
		public static byte[] BuildRandomFile (int size)
		{
			Random r = new Random ();
			byte[] ret = new byte[size];

			r.NextBytes (ret);

			return ret;
		}

		public static string HashBytes (byte[] data)
		{
			HashAlgorithm hash = new SHA256Managed();

			return Convert.ToBase64String (hash.ComputeHash (data));
		}

		public static void EncodeDecodeTest (IEncode enc,
		                                     IDecode dec,
		                                     double passPercent)
		{
			byte[] origFile = enc.Data;
			string origHash = HashBytes (origFile);

			for (int i=0; i<enc.BlocksNeeded; i++)
			{
				dec.Catch (enc.Encode());
			}

			byte[] decFile;
			int numAttempts = 0;
			double decEfficiency;

			while (true)
			{
				// Calculate efficiency, when it drops below a certain amount we will fail the test
				decEfficiency = (double)enc.K / (double)dec.CaughtDroplets;
				decEfficiency *= 100;
				decEfficiency = Math.Round (decEfficiency, 2);

				if (decEfficiency < passPercent)
				{
					Assert.Fail ("Decoding successful, however efficiency was to low: " +
								decEfficiency + "%" + ", wanted: " + passPercent + "%");
				}

				decFile = dec.Decode ();
				if (decFile == null)
				{
					dec.Catch (enc.Encode ());
					numAttempts++;
					continue;
				}

				if (HashBytes (decFile) == origHash)
				{
					Console.WriteLine ("Efficiency: {0}%", decEfficiency);
					break;
				}
				else
				{
					for (int i=0; i<decFile.Length; i++)
					{
						Console.WriteLine ("[{0}]: {1} | {2}", 
						                   i.ToString ("000"),
						                   origFile [i].ToString ("000"),
						                   decFile [i].ToString ("000"));

					}
					Assert.Fail ("Decoding returned data, but hash failed, check console for data dump.");
				}
			}

			// Default, if we exit the loop, we pass the test
			Assert.Pass ();
		}
	}
}

