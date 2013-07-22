using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using LubyTransform;

namespace Tests
{
	[TestFixture()]
	public class MyLT
	{
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

		[Test()]
		public void EncodeDecode ()
		{
			byte[] origFile = BuildRandomFile (1000);
			string origHash = HashBytes (origFile);


			MyEncoder enc = new MyEncoder (origFile, 100, 50);
			MyDecoder dec = new MyDecoder (enc.K, enc.BlockSize, enc.BlocksNeeded);

			for (int i=0; i<enc.BlocksNeeded; i++)
			{
				dec.Catch (enc.BuildBlock ());
			}

			byte[] decFile;

			while (true)
			{
				if (dec.CanTrySolve == false)
				{
					Console.WriteLine ("Adding a drop: {0}", dec.CaughtBlocks);
					dec.Catch (enc.BuildBlock ());
				}
				else
				{
					decFile = dec.Decode ();
					if (HashBytes (decFile) == origHash)
					{
						Console.WriteLine ("Decoded after: {0}", dec.CaughtBlocks);
						break;
					}
				}
			}

			Assert.Pass ();
		}
	}
}

