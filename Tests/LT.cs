using System;
using NUnit.Framework;
using LubyTransform.Transform;
using LubyTransform.Maths;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Tests
{
	[TestFixture()]
	public class LT
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
		public void EncodeTest ()
		{
			byte[] origFile = BuildRandomFile (1000);
			string origHash = HashBytes (origFile);


			IEncode enc = new Encode (origFile, 10);

			List<Drop> drops = new List<Drop> ();
			for (int i=0; i<enc.NumberOfBlocks; i++)
			{
				drops.Add (enc.Encode ());
			}

			byte[] decFile;
			MatrixSolver ms = new MatrixSolver ();
			IDecode dec = new Decode (ms);

			while (true)
			{
				if (dec.Decode (drops, drops.Count, enc.ChunkSize, origFile.Length, out decFile) == true)
				{
					if (HashBytes (decFile) == origHash)
					{
						Console.WriteLine ("Decoded after: {0}", drops.Count);
						break;
					}
				}
			

				Console.WriteLine ("Not enough drops, adding 1 more.");
				drops.Add (enc.Encode ());
				drops.Add (enc.Encode ());
				drops.Add (enc.Encode ());
				drops.Add (enc.Encode ());
				Console.WriteLine ("# Drops: {0}", drops.Count);
			}

			Assert.Pass ();
		}
	}
}

