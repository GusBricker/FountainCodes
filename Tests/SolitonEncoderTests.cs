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
		public static void Test1 ()
		{
			byte[] data = GenericTests.BuildRandomFile (51212);

			SolitonEncoder enc = new SolitonEncoder (data,
			                                         100);
			MyDecoder dec = new MyDecoder (enc.K,
			                               enc.BlockSize,
			                               enc.BlocksNeeded,
			                               enc.Size);

			GenericTests.EncodeDecodeTest (enc,
			                               dec,
			                               60);

		}
	}
}

