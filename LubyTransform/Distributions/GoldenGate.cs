using System;
using System.Collections.Generic;

namespace LubyTransform.Distributions
{
	public class GoldenGate : IDistribution
	{
		private double _maxRate;
		private double _maxDegree;
		private double _lastDegree;
		private int _K;
		private bool _up;
		private int _generatedCount;
		private double _kFactor;

		public GoldenGate (int K,
		                   double maxRate,
		                   double maxDegree,
		                   double kFactor)
		{
			_kFactor = kFactor;
			_generatedCount = 0;
			_K = K;
			_maxRate = maxRate;
			_maxDegree = maxDegree;
			_lastDegree = 1;
			_up = true;
		}

		public int EstimateBlocks 
		{
			get
			{
				return _K;
			}
		}

		public int Degree ()
		{
			Random r = new Random ();

			double v = r.NextDouble ();
			v *= _maxRate;
			// This makes the value blow up easily 
//			v = Math.Exp (v);

			if (_up == true)
			{
				_lastDegree += v;
				if (_lastDegree > _maxDegree)
				{
					_lastDegree = _maxDegree;
					_up = false;
				}
			}
			else
			{
				_lastDegree -= v;
				if (_lastDegree < 1.0)
				{
					_lastDegree = 1.0;
					_up = true;
				}
			}

			double ret = _lastDegree;

			++_generatedCount;
			if (_generatedCount > _K)
			{
				ret *= _kFactor;
				if (ret < 1.0)
				{
					ret = 1.0;
				}
			}

			return (int)ret;
		}
	}
}

