using System;
using System.Security.Cryptography;

namespace LubyTransform.Helpers
{
	public class CryptoRNGHelper
	{
		private RandomNumberGenerator _rng;

		public CryptoRNGHelper ()
		{
			_rng = RNGCryptoServiceProvider.Create ();
		}

		public double NextDouble ()
		{
			byte[] intBytes = new byte[4];
			_rng.GetBytes (intBytes);

			double val = BitConverter.ToUInt32 (intBytes, 0);
			val /= UInt32.MaxValue;

			return val;
		}

		public int Next ()
		{
			return Next (0, Int32.MaxValue);
		}

		public int Next (int max)
		{
			return Next (0, max);
		}

		public int Next (int min, int max)
		{
			double val = NextDouble ();

			return (int)Math.Round(val * (max - min - 1)) + min;
		}
	}
}

