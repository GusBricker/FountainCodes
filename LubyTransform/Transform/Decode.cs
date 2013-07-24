using System.Collections.Generic;
using LubyTransform.Maths;

namespace LubyTransform.Transform
{
    public class Decode : IDecode
    {
        #region Member Variables

        readonly IMatrixSolver matrixSolver;

        #endregion

        #region Constructor

        public Decode(IMatrixSolver matrixSolver)
        {
            this.matrixSolver = matrixSolver;
        }

        #endregion

        #region IDecode

        bool IDecode.Decode(IList<Drop> goblet, int blocksCount, int chunkSize, int fileSize, out byte[] data)
        {
			data = null;
            var matrix = BuildMatrix(goblet, blocksCount, chunkSize);
			if (matrixSolver.Solve (matrix) == false)
			{
				return false;
			}

            int columnsCount = matrix.GetLength(1);
            byte[] result = new byte[fileSize];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)matrix[i, columnsCount - 1];
            }

			data = result;

            return true;
        }

        #endregion

        #region Private Methods

        private int[,] BuildMatrix(IList<Drop> goblet, int blocksCount, int chunkSize)
        {
            var rowsCount = goblet.Count * chunkSize;
            var columnsCount = (blocksCount * chunkSize) + 1;
            var matrix = new int[rowsCount, columnsCount];

            for (int dropIdx = 0; dropIdx < goblet.Count; dropIdx++)
            {
                var drop = goblet[dropIdx];

                for (int i = 0; i < chunkSize; i++)
                {
                    var row = (dropIdx * chunkSize) + i;
                    matrix[row, columnsCount - 1] = drop.Data[i];

                    foreach (var part in drop.SelectedParts)
                    {
                        var col = (part * chunkSize) + i;
                        matrix[row, col] = 1;
                    }
                }
            }

            return matrix;
        }

        #endregion
    }
}