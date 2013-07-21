using System.Collections.Generic;

namespace LubyTransform.Transform
{
    public interface IDecode
    {
        byte[] Decode(IList<Drop> goblet, int blocksCount, int chunkSize, int fileSize);
    }
}