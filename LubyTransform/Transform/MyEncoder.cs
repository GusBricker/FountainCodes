using System;
using System.Collections.Generic;
using LubyTransform.Distributions;
using System.Security.Cryptography;
using System.Text;

namespace LubyTransform.Transform
{
	public class MyEncoder
	{
		public int K { get; private set; }

		public int BlockSize { get; private set; } 	

//		private Soliton _solDist;

		private IDistribution _dist;

		private byte[][] _data;

		private RandomNumberGenerator _neighbourSelector; 
//		private Random _neighbourSelector;

		public int PaddedSize
		{
			get
			{
				return K * BlockSize; 
			}
		}

		public int Size { get; private set; }

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

			Size = length;
			K = (int)Math.Ceiling ((double)length / (double)blockSize);
			BlockSize = blockSize;
//			_neighbourSelector = new Random ((int)DateTime.Now.Ticks);
			_neighbourSelector = RNGCryptoServiceProvider.Create ();

			_dist  = new Soliton (this.K, 0.0012, 0.01);
//			_dist = new GoldenGate (K, 5, BlockSize, 0.5);
//			_dist = new Ramping (K, ((double)1/(double)K)*2, 8);
//			_dist = new Ramping (K, ((double)1/(double)K)*5, 20);
		}

		public int BlocksNeeded
		{
			get
			{
				return _dist.EstimateBlocks;
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
			int d=0, neighbourOffset;

			d = _dist.Degree ();
			encodingBlock = new Droplet(d, BlockSize);

//			for (int a=0; a<d; a++)
//			{
//				Console.Write ("=");
//			}
//			Console.WriteLine ();

			for (int numNeighbours=1; numNeighbours<=d; numNeighbours++)
			{
				// Get a block offset
//				neighbourOffset = _neighbourSelector.Next (K);
				do
				{
					neighbourOffset = NextRandomInt (K);
					// Loop until we generate a neighbour that isn't already in the list
				} while (encodingBlock.Neighbours.Contains (neighbourOffset) == true);
				
				encodingBlock.AddNeighbour(neighbourOffset);

				if (numNeighbours == 1)
				{
					encodingBlock.Data = _data[neighbourOffset];
				}
				else
				{
					XorInto (encodingBlock.Data, _data [neighbourOffset]);
				}
			}

			return (encodingBlock);
		}

		private int NextRandomInt (int max)
		{
			byte[] intBytes = new byte[4];

			_neighbourSelector.GetBytes (intBytes);

			double val = BitConverter.ToUInt32 (intBytes, 0);
			val /= UInt32.MaxValue;

			return (int)Math.Round(val * (max - 1));
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

		private static void XorInto (byte[] target, byte[] data)
		{
			for (int i=0; i<target.Length; i++)
			{
				target [i] ^= data [i];
			}
		}

//		private static void Xor (byte[] input) 
//		{ 
//			if(input.Length >= _data.Length)
//			{
//				for (int i=0; i<_data.Length; i++)
//				{
//					_data[i] = (byte)(_data[i] ^ input[i]);
//				}
//			}
//			else
//			{
//				byte[] aux = new byte[_data.Length];
//				Array.Copy (input, aux, _data.Length);
//
//				for (int i=0; i<_data.Length; i++)
//				{
//					_data[i] = (byte)(_data[i] ^ aux[i]);
//				}
//			}
//		}
	}
}

