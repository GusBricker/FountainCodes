using System;

namespace LubyTransform.Maths
{
    public interface IMatrixSolver
    {
        bool Solve(int[,] matrix);
    }
}
