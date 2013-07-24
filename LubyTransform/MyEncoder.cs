using System;
using System.Collections.Generic;
using LubyTransform.Distributions;
using System.Text;

namespace LubyTransform
{
	public class MyEncoder
	{
		public int K { get; private set; }			// 1000

		public int BlockSize { get; private set; } 	// 32

		private Soliton _solDist;

		private byte[][] _data;

		public MyEncoder (string data, int blockSize) 
		{
			CommonInit (data.Length, blockSize);

			byte[] padded = Pad (Encoding.ASCII.GetBytes (data)); 
			_data = Split (padded);
		}

		public MyEncoder (byte[] data, int blockSize) 
		{
			CommonInit (data.Length, blockSize);

			byte[] padded = Pad (data);
			_data = Split (padded);
		}

		private void CommonInit (int length, int blockSize)
		{
			if (blockSize <= 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			this.K = (int)Math.Ceiling ((double)(length / blockSize));
			this.BlockSize = blockSize;

			try 
			{
				_solDist = new Soliton(this.K, 0.12, 0.01);
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
		public Droplet BuildBlock ()
		{
			Droplet encodingBlock = null;
			Random rGen1 = new Random ();
			Random rGen2 = new Random ();
			int seed;
			int d=0, neighbourOffset;

			seed = rGen1.Next (); 
			d = _solDist.Robust (seed);
			rGen2 = new Random (seed);
			encodingBlock = new Droplet(seed, d, BlockSize);

			for (int numNeighbours=1; numNeighbours<=d; numNeighbours++)
			{
				// Get a block offset
				neighbourOffset = rGen2.Next (K); 
				if (encodingBlock.GetNeighbours ().Contains (neighbourOffset) == true)
				{
					// No point in allowing duplicate neighbours, is just wasted processing time...
					continue;
				}

				encodingBlock.AddNeighbour(neighbourOffset);

				if (numNeighbours == 1)
				{
					encodingBlock.setData(_data[neighbourOffset]);
				}
				else
				{
					encodingBlock.Xor(_data[neighbourOffset]);
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
			byte[] padded = new byte[K * BlockSize];

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
			byte[][] ret = new byte[K][]; 
			int j=0, beginning=0, end=BlockSize;

			for(int i = 0; i < K; i++, j=0)
			{
				ret[i] = new byte[BlockSize];

				for(int k=beginning; k<end; k++, j++){
					ret[i][j] = padded[k];
				}

				beginning += BlockSize;
				end += BlockSize;
			}

			return ret;
		}	
	}
}

