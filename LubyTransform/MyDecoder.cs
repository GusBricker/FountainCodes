using System;
using System.Collections.Generic;

namespace LubyTransform
{
	public class MyDecoder
	{
		private Dictionary<uint, List<Droplet>> _drops;
		public int CaughtDrops { get; private set; }
		private uint _maxCaughtDegree;
		private uint _minCaughtDegree;

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
			_minCaughtDegree = UInt32.MaxValue;
			_maxCaughtDegree = 0;
			_originalSize = originalSize;
			_K = k;
			_blockSize = blockSize;
			_blocksNeeded = blocksNeeded;
			_drops = new Dictionary<uint, List<Droplet>>();
			_decodedData = new byte[_K][];
			CaughtDrops = 0;

			for (int i=0; i<_K; i++)
			{
				_decodedData [i] = null;
			}
		}

		public void Catch (Droplet block)
		{
			uint degree = (uint)block.Neighbours.Count;

			if (degree > _maxCaughtDegree)
			{
				_maxCaughtDegree = degree;
			}

			if (degree < _minCaughtDegree)
			{
				_minCaughtDegree = degree;
			}

			if (_drops.ContainsKey (degree) == false)
			{
				_drops.Add (degree, new List<Droplet> ());
			}
			_drops [degree].Add (block);
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
			List<Droplet> dropList;

//			PrintDrops ();

			for (uint degree=_minCaughtDegree; degree<=_maxCaughtDegree; degree++)
			{
				if (_drops.ContainsKey (degree) == false)
				{
					continue;
				}

				dropList = _drops [degree];
//				while (dropQueue.Count >= 1)
				for (int dropletIndex=0; dropletIndex<dropList.Count; dropletIndex++)
				{
					d = dropList [dropletIndex];
					neighbours = d.Neighbours;

					if (neighbours.Count > 1)
					{
						if (Solve (d, neighbours) == false)
						{
							// Isnt useful anymore
							dropList.RemoveAt (dropletIndex);
						}

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
				_drops.Remove (degree);
			}

//			PrintProgress ();

			_minCaughtDegree = UInt32.MaxValue;
			_maxCaughtDegree = 0;

			return Merge (_decodedData);
		}

		private void PrintProgress ()
		{
			for (int i=0; i<_K; i++)
			{
				if (_decodedData[i] == null)
				{
					Console.WriteLine ("[{0}]: N", i);
				}
				else
				{
					Console.WriteLine ("[{0}]: Y", i);
				}
			}
		}

		private void PrintDrops ()
		{
			List<Droplet> dropList;

			for (uint degree=_minCaughtDegree; degree<=_maxCaughtDegree; degree++)
			{
				if (_drops.ContainsKey (degree) == false)
				{
					continue;
				}

				dropList = _drops [degree];
				foreach (Droplet d in dropList)
				{
					Console.Write ("D: {0}, N: ", d.Neighbours.Count);
					for (int i=0; i<d.Neighbours.Count; i++)
					{
						Console.Write ("{0},", d.Neighbours [i]);
					}
					Console.WriteLine ();
				}
			}
		}

		private bool Solve (Droplet d, List<int> neighbours)
		{
			int solvingFor;
			int neighbourIndex;
			bool isUseful = false;

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
							// We cant solve, bomb out
							_decodedData [solvingFor] = null;
							isUseful = true;
							break;
						}
						XorInto (_decodedData [solvingFor], _decodedData [neighbourIndex]);
					}
				}
			}

			return isUseful;
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

