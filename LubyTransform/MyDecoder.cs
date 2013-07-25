using System;
using System.Collections.Generic;

namespace LubyTransform
{
	public class MyDecoder
	{
		private Queue<Droplet> _dropQueue;
		public int CaughtDrops { get; private set; }

		private int _blocksNeeded;
		private int _K;
		private int _blockSize;
		private byte[][] _decodedData;
		private int _originalSize;

		public MyDecoder (int k, 
		                  int blockSize, 
		                  int blocksNeeded,
		                  int originalSize)
		{
			_originalSize = originalSize;
			_K = k;
			_blockSize = blockSize;
			_blocksNeeded = blocksNeeded;
			_dropQueue = new Queue<Droplet>();
			_decodedData = new byte[_K][];
			CaughtDrops = 0;

			for (int i=0; i<_K; i++)
			{
				_decodedData [i] = null;
			}
		}

		public void Catch (Droplet block)
		{
			_dropQueue.Enqueue (block);
			CaughtDrops++;
		}

		public bool CanTrySolve 
		{
			get
			{
				if (CaughtDrops < _blocksNeeded)
				{
					return false;
				}

				return true;
			}
		}

		/**
		 * Decodes the blocks in <code>buffer</code>.
		 * @return <code>Byte array</code> representing the decoded message.
		 */
		public byte[] Decode ()
		{
			List<int> neighbours;
			int neighbourOffset;
			Droplet d;

			while (_dropQueue.Count >= 1)
			{
				d = _dropQueue.Dequeue ();
				neighbours = d.Neighbours;

				if (neighbours.Count > 1)
				{
					Solve (d, neighbours);
				}
				else
				{
					neighbourOffset = neighbours[0];
					if (_decodedData[neighbourOffset] == null)
					{
						_decodedData[neighbourOffset] = new byte[_blockSize];

						// No neighbours? Means the actual data is contained in this droplet
						Array.Copy (d.Data, _decodedData[neighbourOffset], _blockSize);
					}
				}
			}

			return Merge (_decodedData);
		}

		private void Solve (Droplet d, List<int> neighbours)
		{
			int solvingFor;
			int neighbourIndex;

			for (int index=0; index<neighbours.Count; index++)
			{
				solvingFor = neighbours [index];

				if (_decodedData [solvingFor] == null)
				{
					// Setup the block we are about to decode
					_decodedData [solvingFor] = new byte[_blockSize];
					Array.Copy (d.Data, _decodedData[solvingFor], _blockSize);

					// Try solve this one
					for (int othersIndex=0; othersIndex<neighbours.Count; othersIndex++)
					{
						if (othersIndex == index)
						{
							continue;
						}
						neighbourIndex = neighbours [othersIndex];

						if (_decodedData [neighbourIndex] == null)
						{
							// We can solve, bomb out
							_decodedData [solvingFor] = null;
							break;
						}
						XorInto (_decodedData [solvingFor], _decodedData [neighbourIndex]);
					}
				}
			}
		}

		private static void XorInto (byte[] target, byte[] data)
		{
			for (int i=0; i<target.Length; i++)
			{
				target [i] ^= data [i];
			}
		}

//		public byte[] Decode ()
//		{
//			Droplet i;
//			int j;
//
//			while ((i = ExistsNeighbours(_caughtBlocks)) != null)
//			{ 
//				// If does not exist, and ripple is empty, then it's a fail.
//				j = i.GetNeighbours () [0];
//
//				_decodedData [j] = i.Data ();
//
//				foreach (Droplet l in _caughtBlocks)
//				{
//					if (l.GetNeighbours().Contains(j) == true)
//					{
//						l.Xor(_decodedData[j]);
//						l.RemoveNeighbour(j);
//					}
//				}
//				_caughtBlocks.Remove (i);
//			}
//
//			return(Merge(_decodedData));
//		}

		/**
		 * Merges a matrix into an <code>array</code>
		 * @param inputBlock Matrix to be merged.
		 * @return Merged <code>array</code>.
		 */
		private byte[] Merge(byte[][] inputBlock){

			int blocks = inputBlock.Length;
			List<byte> ret = new List<byte> ();
			int upTo = 0;

			for (int i=0; i<inputBlock.Length; i++)
			{
				if (inputBlock [i] == null)
				{
					return null;
				}

				if ((_originalSize - upTo) > _blockSize)
				{
					ret.AddRange (inputBlock [i]);
				}
				else
				{
					// Last block
					for (int j=0; j<_originalSize-upTo; j++)
					{
						ret.Add (inputBlock[i][j]);
					}
					break;
				}
				upTo += _blockSize;
			}

//			for (int i = 0, ini = 0; i < blocks; i++, ini += _blockSize)
//			{
//				if (inputBlock [i] == null)
//				{
//					return null;
//				}
//
//				for (int j = 0; j < _blockSize; j++)
//				{
//					ret[ini + j] = inputBlock[i][j];
//				}
//			}
			return ret.ToArray();
		}

	}
}

