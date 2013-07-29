using System;
using System.Collections.Generic;
using LubyTransform.Distributions;
using System.Security.Cryptography;
using System.Text;
using LubyTransform.Helpers;

namespace LubyTransform.Transform
{
	public class SolitonEncoder : IEncode
	{
		private IDistribution _dist;

		private byte[][] _data;

		private CryptoRNGHelper _neighbourSelector; 

		public SolitonEncoder (byte[] data, int blockSize) 
		{
			if (blockSize <= 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			Data = data;
			Size = data.Length;
			K = (int)Math.Ceiling ((double)data.Length / (double)blockSize);
			BlockSize = blockSize;
			_neighbourSelector = new CryptoRNGHelper ();

			_dist = new Soliton (this.K, 0.0012, 0.0001);

			byte[] padded = Pad (data);
			_data = Split (padded);
		}

		public int PaddedSize
		{
			get
			{
				return K * BlockSize; 
			}
		}

		public int Size { get; private set; }

		public byte[] Data { get; private set; } 

		public int K { get; private set; }

		public int BlockSize { get; private set; } 	

		public int BlocksNeeded
		{
			get
			{
				return _dist.EstimateBlocks;
			}
		}

		public Droplet Encode ()
		{
			Droplet encodingBlock = null;
			int d=0, neighbourOffset;

			d = _dist.Degree ();
			encodingBlock = new Droplet(d, BlockSize);

			for (int numNeighbours=1; numNeighbours<=d; numNeighbours++)
			{
				// Get a block offset
				do
				{
					neighbourOffset = _neighbourSelector.Next (K);
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

		private byte[] Pad(byte[] msg)
		{
			byte[] padded = new byte[K * BlockSize];

			Array.Clear (padded, msg.Length, padded.Length - msg.Length);
			Array.Copy (msg, padded, msg.Length);

			return padded;
		}

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
	}
}

