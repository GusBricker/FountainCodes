using System;
using System.Collections.Generic;

namespace LubyTransform.Transform
{
	public class Decoder : IDecode
	{
		private Dictionary<uint, List<Droplet>> _drops;
		private uint _maxCaughtDegree;
		private uint _minCaughtDegree;

		private int _blocksNeeded;
		private int _K;
		private int _blockSize;
		private byte[][] _decodedData;
		private int _originalSize;
		private const bool _debug = false;

		public Decoder (int k, 
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
			CaughtDroplets = 0;

			for (int i=0; i<_K; i++)
			{
				_decodedData [i] = null;
			}
		}

		public int CaughtDroplets { get; private set; }

		public void Catch (Droplet block)
		{
			uint degree = (uint)block.Degree;

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

			List<Droplet> dropList = _drops [degree];
			for (int dropIndex=0; dropIndex<dropList.Count; dropIndex++)
			{
				if (dropList[dropIndex].Equals (block) == true)
				{
					// If any drops the same, dont even bother putting in list
					return;
				}
			}

			_drops [degree].Add (block);
			CaughtDroplets++;
		}

		public bool CanTrySolve 
		{
			get
			{
				if (CaughtDroplets < _blocksNeeded)
				{
					return false;
				}

				return true;
			}
		}

		public byte[] Decode ()
		{
			List<int> neighbours;
			int neighbourOffset;
			Droplet d;
			List<Droplet> dropList;
			int solved;
			int totalSolved;

			do
			{
				totalSolved = 0;

				for (uint degree=_minCaughtDegree; degree<=_maxCaughtDegree; degree++)
				{
					if (_drops.ContainsKey (degree) == false)
					{
						continue;
					}

					dropList = _drops [degree];
					for (int dropletIndex=0; dropletIndex<dropList.Count; dropletIndex++)
					{
						d = dropList [dropletIndex];
						neighbours = d.Neighbours;

						if (_debug == true)
						{
							PrintDrop (d);
						}

						if (neighbours.Count > 1)
						{
							if (Solve (d, neighbours, out solved) == false)
							{
								// Isnt useful anymore
								dropList.RemoveAt (dropletIndex);
								dropletIndex--;
								if (_debug == true)
								{
									Console.WriteLine ("T");
								}
							}
							totalSolved += solved;
						}
						else
						{
							neighbourOffset = neighbours [0];
							if (_decodedData [neighbourOffset] == null)
							{
								_decodedData [neighbourOffset] = new byte[_blockSize];
								++totalSolved;

								// No neighbours? Means the actual data is contained in this droplet
								Array.Copy (d.Data, _decodedData [neighbourOffset], _blockSize);
							}
							dropList.RemoveAt (dropletIndex);
						}
					}

					if (_debug == true)
					{
						PrintProgress ();
					}

					if (dropList.Count == 0)
					{
						_drops.Remove (degree);
					}
				}
			} while (totalSolved > 0);


			if (_drops.Count == 0)
			{
				_minCaughtDegree = UInt32.MaxValue;
				_maxCaughtDegree = 0;
			}

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

		private void PrintDrop (Droplet d)
		{
			Console.Write ("> D: {0}, N: ", d.Neighbours.Count);
			for (int i=0; i<d.Neighbours.Count; i++)
			{
				Console.Write ("{0},", d.Neighbours [i]);
			}
			Console.WriteLine ();
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

		private bool Solve (Droplet d, List<int> neighbours, out int solved)
		{
			int solvingFor;
			int neighbourIndex;
			bool isUseful = false;
			bool didSolve;
			solved = 0;

			for (int index=0; index<neighbours.Count; index++)
			{
				solvingFor = neighbours [index];

				if (_decodedData [solvingFor] == null)
				{
					// Setup the block we are about to decode
					_decodedData [solvingFor] = new byte[_blockSize];
					Array.Copy (d.Data, _decodedData[solvingFor], _blockSize);
					didSolve = true;

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
							didSolve = false;
							break;
						}
						XorInto (_decodedData [solvingFor], _decodedData [neighbourIndex]);
					}
					if (didSolve == true)
					{
						solved++;
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

			return ret.ToArray();
		}

	}
}

