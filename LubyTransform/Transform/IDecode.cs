using System.Collections.Generic;

namespace LubyTransform.Transform
{
    public interface IDecode
    {
        bool Decode(IList<Drop> goblet, int blocksCount, int chunkSize, int fileSize, out byte[] data);
    }
}