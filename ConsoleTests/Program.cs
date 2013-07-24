using System;
using LubyTransform;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ConsoleTests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			EncodeDecode ();
		}

		private static byte[] BuildRandomFile (int size)
		{
			Random r = new Random ();
			byte[] ret = new byte[size];

			r.NextBytes (ret);

			return ret;
		}

		private static string HashBytes (byte[] data)
		{
			HashAlgorithm hash = new SHA256Managed();

			return Convert.ToBase64String (hash.ComputeHash (data));
		}

		public static void EncodeDecode ()
		{
			byte[] origFile = BuildRandomFile (100000);
			string origHash = HashBytes (origFile);


			MyEncoder enc = new MyEncoder (origFile, 10000);
			MyDecoder dec = new MyDecoder (enc.K, enc.BlockSize, enc.BlocksNeeded);
			byte[] decFile;

			while (true)
			{
				dec.Catch (enc.BuildBlock ());

				decFile = dec.Decode ();
				if (decFile == null)
				{
					continue;
				}

				if (HashBytes (decFile) == origHash)
				{
					Console.WriteLine ("Decoded after: {0}, required: {1}", 
					                   dec.CaughtDrops,
					                   enc.BlocksNeeded);
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
				}
			}
		}
	}
}
