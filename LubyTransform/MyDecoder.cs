using System;
using System.Collections.Generic;

namespace LubyTransform
{
	public class MyDecoder
	{
		private List<Droplet> _caughtBlocks;

		private int _blocksNeeded;
		private int _K;
		private int _blockSize;

		public MyDecoder (int k, int blockSize, int blocksNeeded)
		{
			_K = k;
			_blockSize = blockSize;
			_blocksNeeded = blocksNeeded;
			_caughtBlocks = new List<Droplet>();
		}

		public void Catch (Droplet block)
		{
			_caughtBlocks.Add (block);
		}

		public int CaughtBlocks
		{
			get
			{
				return _caughtBlocks.Count;
			}
		}

		public bool CanTrySolve 
		{
			get
			{
				if (_caughtBlocks.Count < _blocksNeeded)
				{
					return false;
				}

				if (ExistsExactlyOneNeighbour (_caughtBlocks) == null)
				{
					// If theres a ripple, we can not solve
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
			Droplet i;
			int j;
			byte[][] inputBlock = new byte[_K][];
			while ((i = ExistsExactlyOneNeighbour(_caughtBlocks)) != null)
			{ // If does not exist, and ripple is empty, then it's a fail.
				j = i.GetNeighbours () [0];

				inputBlock [j] = i.Data ();

				foreach (Droplet l in _caughtBlocks)
				{
					if (l.GetNeighbours().Contains(j) == true)
					{
						l.Xor(inputBlock[j]);
						l.RemoveNeighbour(j);
					}
				}
			}

			return(Merge(inputBlock));
		}

		/**
		 * Merges a matrix into an <code>array</code>
		 * @param inputBlock Matrix to be merged.
		 * @return Merged <code>array</code>.
		 */
		private byte[] Merge(byte[][] inputBlock){

			int blocks = inputBlock.Length;
			byte[] ret = new byte[blocks * _blockSize];

			for (int i = 0, ini = 0; i < blocks; i++, ini += _blockSize)
				for (int j = 0; j < _blockSize; j++)
					ret[ini + j] = inputBlock[i][j];

			return ret;
		}

		/**
		 * Checks the elements of a given list, if there is one with a single connection to the source packets.
		 * @param list List of <code>Block</code>s.
		 * @return The first <code>Block</code> found, with only one connection to the source packets. 
		 * <code>null</code> if no <code>Block</code> met the condition.
		 */
		private Droplet ExistsExactlyOneNeighbour(List<Droplet> list){

			if (list == null || list.Count == 0)
			{
				return null;
			}

			foreach (Droplet b in list)
			{
				if (b.GetNeighbours ().Count == 1)
				{
					return b;
				}
			}

			return null;
		}
	}
}

