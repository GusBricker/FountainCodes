using System;
using NUnit.Framework;
using LubyTransform.Transform;

namespace Tests
{
	[TestFixture()]
	public class TrackingEncoderTests
	{
		[Test()]
		public static void LargeFileSmallBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312964);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           10);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               80);

		}

		[Test()]
		public static void MediumFileSmallBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (51212);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           10);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               80);

		}

		[Test()]
		public static void SmallFileSmallBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           10);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               75);

		}

		[Test()]
		public static void LargeFileMediumBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312964);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           100);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               80);

		}

		[Test()]
		public static void MediumFileMediumBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (51212);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           100);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               80);

		}

		[Test()]
		public static void SmallFileMediumBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           100);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               75);

		}
		[Test()]
		public static void LargeFileLargeBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312964);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           1000);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               80);

		}

		[Test()]
		public static void MediumFileLargeBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (51212);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           1000);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               80);

		}

		[Test()]
		public static void SmallFileLargerBlock ()
		{
			byte[] data = GenericTests.BuildRandomFile (312);

			TrackingEncoder enc = new TrackingEncoder (data,
			                                           150);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               75);

		}

	}
}

