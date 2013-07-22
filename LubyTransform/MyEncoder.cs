using System;
using System.Collections.Generic;
using LubyTransform.Distributions;
using System.Text;

namespace LubyTransform
{
	public class MyEncoder
	{
		private int _K;							// 1000

		private int _blockSize;					// 32

		private Soliton _solDist;

		private byte[][] _data;

		public MyEncoder (string data, int k, int blockSize) 
		{
			byte[] padded = Pad (Encoding.ASCII.GetBytes (data)); 
			_data = Split (padded);

			CommonInit (k, blockSize);
		}

		public MyEncoder (byte[] data, int k, int blockSize) 
		{
			byte[] padded = Pad (data);
			_data = Split (padded);

			CommonInit (k, blockSize);
		}

		private void CommonInit (int k, int blockSize)
		{
			if (k <= 0 || blockSize <= 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			this._K = k;
			this._blockSize = blockSize;

			try 
			{
				_solDist = new Soliton(this._K, 0.12, 0.01);
			}
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
			}
		}

		public int BlocksNeeded
		{
			get
			{
				return _solDist.BlocksNeeded;
			}
		}

		/**
	 * Encodes a given <code>byte array</code>.
	 * @param inputBlock <code>Byte array</code> to be encoded.
	 * @return List of encoded <code>Block</code>s.
	 * @throws NullPointerException In case the input <code>array</code> is null.
	 */
		public Drop BuildBlock ()
		{
			Drop encodingBlock = null;
			Random rGen1 = new Random ();
			Random rGen2 = new Random ();
			int seed;
			int d=0, j;

			seed = rGen1.Next (); 
			d = _solDist.Robust (seed);
			rGen2 = new Random(seed);
			encodingBlock = new Drop(seed, d, _blockSize);

			for(int x=1; x<=d; x++)
			{
				j = rGen2.Next (_K); // TODO: j <- random(1,k) -- inclusive?

				encodingBlock.AddNeighbour(j);

				if(x == 1)
				{
					encodingBlock.setData(_data[j]);
				}
				else
				{
					encodingBlock.Xor(_data[j]);
				}
			}

			return (encodingBlock);
		}

		/**
	 * Pads a <code>byte array</code> with '0's, if needed.
	 * @param msg <code>Byte array</code> to be padded.
	 * @return Padded <code>byte array</code>.
	 */
		public byte[] Pad(byte[] msg)
		{
			byte[] padded = new byte[_K * _blockSize];

			Array.Clear (padded, msg.Length, padded.Length - msg.Length);
			Array.Copy (msg, padded, msg.Length);

			return padded;
		}

		/**
	 * Splits a <code>byte array</code> into a matrix.
	 * @param padded <code>Byte array</code> to be split.
	 * @return Split matrix.
	 */
		private byte[][] Split(byte[] padded)
		{
			byte[][] ret = new byte[_K][]; 
			int j=0, beginning=0, end=_blockSize;

			for(int i = 0; i < _K; i++, j=0)
			{
				ret[i] = new byte[_blockSize];

				for(int k=beginning; k<end; k++, j++){
					ret[i][j] = padded[k];
				}

				beginning += _blockSize;
				end += _blockSize;
			}

			return ret;
		}	
	}
}

