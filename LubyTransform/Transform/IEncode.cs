using System.Collections.Generic;

namespace LubyTransform.Transform
{
    public interface IEncode
    {
		int K { get; } 
		int BlockSize { get; }

		int PaddedSize { get; }
		int Size { get; }
		byte[] Data { get; }

		int BlocksNeeded { get; }
		Droplet Encode ();
    }
}