using System.Collections.Generic;

namespace LubyTransform.Transform
{
    public interface IDecode
    {
		void Catch (Droplet drop);
		byte[] Decode ();

		bool CanTrySolve { get; }
		int CaughtDroplets { get; }
    }
}