using System;
using System.Collections.Generic;

namespace LubyTransform.Transform
{
	public class MyDecoder2
	{
		private Dictionary<uint, List<Droplet>> _drops;
		public int CaughtDrops { get; private set; }
		private uint _maxCaughtDegree;
		private uint _minCaughtDegree;

		private int _blocksNeeded;
		private int _K;
		private int _blockSize;
		private int _originalSize;

		private byte[][] _decodedData;

		public MyDecoder2 (int k, 
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
			CaughtDrops = 0;

			_decodedData = new byte[_K][];
			for (int i=0; i<_K; i++)
			{
				_decodedData [i] = null;
			}

			_drops = new Dictionary<uint, List<Droplet>> ();
		}

		public void Catch (Droplet drop)
		{
			uint degree = (uint)drop.Degree;

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
				if (dropList[dropIndex].Equals (drop) == true)
				{
					// If any drops the same, dont even bother putting in list
					return;
				}
			}

			dropList.Add (drop);
		}

	}
}

