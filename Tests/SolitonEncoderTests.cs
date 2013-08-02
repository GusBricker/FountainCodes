using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using LubyTransform.Transform;

namespace Tests
{
	[TestFixture()]
	public class SolitonEncoderTests
	{
		[Test()]
		public static void LargeFileSmallBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312964);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         10);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               50);

		}

		[Test()]
		public static void MediumFileSmallBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (51212);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         10);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               65);

		}

		[Test()]
		public static void SmallFileSmallBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         10);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               50);

		}

		[Test()]
		public static void LargeFileMediumBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312964);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         100);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               60);

		}

		[Test()]
		public static void MediumFileMediumBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (51212);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         100);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               60);

		}

		[Test()]
		public static void SmallFileMediumBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         100);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               50);

		}
		[Test()]
		public static void LargeFileLargeBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312964);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         1000);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               60);

		}

		[Test()]
		public static void MediumFileLargeBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (51212);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         1000);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               50);

		}

		[Test()]
		public static void SmallFileLargerBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         150);
			Decoder dec = new Decoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               50);

		}
	}
}

