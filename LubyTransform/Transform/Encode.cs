using System;
using System.Collections.Generic;
using System.Linq;
using LubyTransform.Distributions;

namespace LubyTransform.Transform
{
    public class Encode : IEncode
    {
        #region Member Variables

        private IList<byte[]> _blocks;
        private int _degree;
        private Random _rand;
//		private Soliton _randSol;
        private int _fileSize;
		private int _chunkSize;

        #endregion

        #region Constructor

        public Encode(byte[] file)
        {
			_chunkSize = 2;
			CommonInit (file);
        }

        public Encode(byte[] file, int chunkSize)
		{
			_chunkSize = chunkSize;
			CommonInit (file);
		}

        #endregion

        #region IEncode

        Drop IEncode.Encode()
        {
            int[] selectedParts = GetSelectedParts();
            byte[] data;

            if (selectedParts.Length > 1)
            {
                data = CreateDropData(selectedParts, _blocks, _chunkSize);
            }
            else
            {
                data = _blocks[selectedParts[0]];
            }

            return new Drop { SelectedParts = selectedParts, Data = data };
        }

        int IEncode.NumberOfBlocks
        {
            get { return _blocks.Count; }
        }

        int IEncode.ChunkSize
        {
            get { return _chunkSize; }
        }

        int IEncode.FileSize
        {
            get { return _fileSize; }
        }

        #endregion

        #region Private Methods

		private void CommonInit (byte[] file)
		{
            _rand = new Random();
            _fileSize = file.Length;
            _blocks = CreateBlocks(file);
            _degree = _blocks.Count / 2;
            _degree += 2;
//			_randSol = new Soliton (_blocks.Count, 5, 0.5);
		}

        private IList<byte[]> CreateBlocks(byte[] file)
        {
            var size = _chunkSize;
            var blocksCount = Math.Ceiling((decimal)file.Length / size);
            var remainingSize = file.Length;
            var blocks = new List<byte[]>();

            for (int i = 0; i < blocksCount; i++)
            {
                if (remainingSize > size)
                {
                    remainingSize -= size;
                }
                else
                {
                    size = remainingSize;
                }

                var block = file.Skip(i * _chunkSize).Take(size).ToArray();

                if (block.Length >= _chunkSize)
                {
                    blocks.Add(block);
                }
                else
                {
                    var chunk = new byte[_chunkSize];
                    Array.Copy(block, 0, chunk, 0, block.Length);
                    blocks.Add(chunk);
                }
            }

            return blocks;
        }

        private int[] GetSelectedParts()
        {
            int length = _rand.Next(1, _degree);
//			int length = _randSol.Robust (_degree);
            var selectedParts = new Dictionary<int, int>();
            for (int j = 0; j < length; j++)
            {
                while (true)
                {
                    var part = _rand.Next(_blocks.Count());
//					int part = _randSol.Robust (_blocks.Count ());
                    if (!selectedParts.ContainsKey(part))
                    {
                        selectedParts.Add(part, part);
                        break;
                    }
                }
            }
            return selectedParts.Keys.ToArray();
        }

        private byte[] CreateDropData(IList<int> selectedParts, IList<byte[]> blocks, int chunkSize)
        {
            var data = new byte[chunkSize];

            for (int i = 0; i < chunkSize; i++)
            {
                data[i] = XOROperation(i, selectedParts, blocks);
            }

            return data;
        }

        private byte XOROperation(int idx, IList<int> selectedParts, IList<byte[]> blocks)
        {
            var selectedBlock = blocks[selectedParts[0]];
            byte result = selectedBlock[idx];

            for (int i = 1; i < selectedParts.Count; i++)
            {
                result ^= blocks[selectedParts[i]][idx];
            }

            return result;
        }


        #endregion
    }
}