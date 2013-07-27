using System;

namespace LubyTransform.Distributions
{
	public class Ramping : IDistribution
	{
		private double _maxRate;
		private double _maxDegree;
		private int _K;
		private double _lastDegree;

		public Ramping (int K,
		                double maxRate,
		                double maxDegree)
		{
			_maxRate = maxRate;
			_maxDegree = maxDegree;
			_lastDegree = 1.0;
		}

		public int Degree (int seed)
		{
//			Random r = new Random (seed);
//
//			double v = r.NextDouble ();
//			v *= _maxRate;
//
//			_lastDegree += v;
			_lastDegree += _maxRate;
			if (_lastDegree > _maxDegree)
			{
				_lastDegree = 1.0;
			}

			return (int) _lastDegree;
		}

		public int EstimateBlocks
		{
			get
			{
				return _K;
			}
		}
	}
}

