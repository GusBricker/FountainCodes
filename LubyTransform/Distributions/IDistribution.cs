using System;

namespace LubyTransform.Distributions
{
	public interface IDistribution
	{
		int Degree ();
		int EstimateBlocks { get; } 
	}
}

