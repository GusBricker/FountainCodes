using System.Collections.Generic;

namespace LubyTransform.Transform
{
    public interface IEncode
    {
        Drop Encode();

        int NumberOfBlocks { get; }

        int ChunkSize { get; }

        int FileSize { get; }
    }
}