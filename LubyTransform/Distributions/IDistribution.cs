using System;

namespace LubyTransform.Distributions
{
	public interface IDistribution
	{
		int Degree (int seed);
		int EstimateBlocks { get; } 
	}
}

