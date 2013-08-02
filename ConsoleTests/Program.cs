using System;
using LubyTransform;
using System.Collections.Generic;
using System.Security.Cryptography;
using LubyTransform.Distributions;
using LubyTransform.Transform;
using LubyTransform.Helpers;

namespace ConsoleTests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
//			Console.WriteLine ("Hello World!");
//			EncodeDecode ();
			EncodeDecodeWithLoss ();
//			DisplaySoliton (40);
//			DisplayGoldenGate (40);
		}

		private static void DisplayGoldenGate (int count)
		{
			GoldenGate g = new GoldenGate (count/2, 4.8, 10, 4);
			int max;

			for (int i=0; i<count; i++)
			{
				max = g.Degree ();

				for (int a=0; a<max; a++)
				{
					Console.Write ("=");
				}
				Console.WriteLine ();
			}
		}

		private static void DisplaySoliton (int count)
		{
			Soliton s = new Soliton (count, 0.01, 0.01);
			int max;

			for (int i=0; i<count; i++)
			{
				max = s.Degree ();

				for (int a=0; a<max; a++)
				{
					Console.Write ("=");
				}
				Console.WriteLine ();
			}
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

		public static void EncodeDecodeWithLoss ()
		{
			const int successPercent = 90;
			byte[] origFile = BuildRandomFile (5112);
			string origHash = HashBytes (origFile);
			int generatedDroplets;


			IEncode enc = new TrackingEncoder (origFile, 100);
//			IEncode enc = new SolitonEncoder (origFile, 100);
			MyDecoder dec = new MyDecoder (enc.K, 
			                               enc.BlockSize, 
			                               enc.BlocksNeeded,
			                               enc.Size);
			byte[] decFile;
			Droplet droplet;
			generatedDroplets = 0;

			for (int i=0; i<enc.K; i++)
			{
				droplet = enc.Encode ();
				++generatedDroplets;

				if (WeightedDiceRoll (successPercent))
				{
					dec.Catch (droplet);
				}
			}

			while (true)
			{
				decFile = dec.Decode ();
				if (decFile == null)
				{
					droplet = enc.Encode ();
					++generatedDroplets;

					if (WeightedDiceRoll (successPercent))
					{
						dec.Catch (droplet);
					}
					continue;
				}

				if (HashBytes (decFile) == origHash)
				{
					Console.WriteLine ("Decoded after: {0}, generated: {1}, K: {2}, blocksize: {3}", 
					                   dec.CaughtDroplets,
					                   generatedDroplets,
					                   enc.K,
					                   enc.BlockSize);

					double efficiency;
					efficiency = (double)dec.CaughtDroplets / (double)generatedDroplets;
					efficiency *= 100.0;
					efficiency = Math.Round (efficiency, 2);
					Console.WriteLine ("Overhead Efficiency: {0}%", efficiency); 

					efficiency = (double)enc.BlocksNeeded / (double)dec.CaughtDroplets;
					efficiency *= 100.0;
					efficiency = Math.Round (efficiency, 2);
					Console.WriteLine ("Efficiency: {0}%", efficiency); 
					Console.WriteLine ("Exiting");
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

		public static void EncodeDecode ()
		{
			byte[] origFile = BuildRandomFile (51212);
			string origHash = HashBytes (origFile);


			TrackingEncoder enc = new TrackingEncoder (origFile, 100);
			MyDecoder dec = new MyDecoder (enc.K, 
			                               enc.BlockSize, 
			                               enc.BlocksNeeded,
			                               enc.Size);
			byte[] decFile;

			for (int i=0; i<enc.K; i++)
			{
				dec.Catch (enc.Encode ());
			}

			while (true)
			{
				decFile = dec.Decode ();
				if (decFile == null)
				{
					dec.Catch (enc.Encode ());
					continue;
				}

				if (HashBytes (decFile) == origHash)
				{
					Console.WriteLine ("Decoded after: {0}, required: {1}, K: {2}, blocksize: {3}", 
					                   dec.CaughtDroplets,
					                   enc.BlocksNeeded,
					                   enc.K,
					                   enc.BlockSize);
					Console.WriteLine ("Efficiency: {0}", (((double)enc.K / (double)dec.CaughtDroplets)).ToString ("000.0%"));
					Console.WriteLine ("Target Efficiency: {0}", (((double)enc.BlocksNeeded / (double)dec.CaughtDroplets)).ToString ("000.0%"));
					Console.WriteLine ("Exiting");
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

		static CryptoRNGHelper diceRoller = new CryptoRNGHelper ();
		private static bool WeightedDiceRoll(int headsWeightingPercent)
		{
			int decision;

			decision = diceRoller.Next (0, 100);

			return decision < headsWeightingPercent;
		}
	}
}
