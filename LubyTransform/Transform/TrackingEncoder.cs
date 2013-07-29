using System;
using System.Collections.Generic;
using LubyTransform.Distributions;
using System.Security.Cryptography;
using System.Text;

namespace LubyTransform.Transform
{
	public class TrackingEncoder : IEncode
	{
		private Tracking _trackingDist;

		private byte[][] _data;

		public TrackingEncoder (byte[] data, int blockSize) 
		{
			if (blockSize <= 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			Data = data;
			Size = data.Length;
			K = (int)Math.Ceiling ((double)data.Length / (double)blockSize);
			BlockSize = blockSize;

			_trackingDist = new Tracking (K,
			                              95,
			                              100);

			byte[] padded = Pad (data);
			_data = Split (padded);
		}

		public int K { get; private set; }

		public int BlockSize { get; private set; } 	

		public int PaddedSize
		{
			get
			{
				return K * BlockSize; 
			}
		}

		public int Size { get; private set; }

		public byte[] Data { get; private set; } 

		public int BlocksNeeded
		{
			get
			{
				return _trackingDist.EstimateBlocks;
			}
		}

		public Droplet Encode ()
		{
			Droplet encodingBlock = null;
			int d = 0;
			int[] generatedNeighbours;
			int currentNeighbour;

			_trackingDist.Generate ();
			d = _trackingDist.Degree ();
			generatedNeighbours = _trackingDist.Neighbours ();

			encodingBlock = new Droplet(d, BlockSize);

			for (int numNeighbours=0; numNeighbours<d; numNeighbours++)
			{
				currentNeighbour = generatedNeighbours [numNeighbours];
				encodingBlock.AddNeighbour(currentNeighbour);

				if (numNeighbours == 0)
				{
					encodingBlock.Data = _data[currentNeighbour];
				}
				else
				{
					XorInto (encodingBlock.Data, _data [currentNeighbour]);
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

