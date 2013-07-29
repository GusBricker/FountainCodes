using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using LubyTransform.Helpers;

namespace LubyTransform.Distributions
{
	public class Tracking : IDistribution
	{
		private int _K;
		private List<int> _generatedNeighboursCounter;
		private int _rampPoint;
		private int _rampedMaxDegree;
		private CryptoRNGHelper _rampedDegreeSelector;
		private CryptoRNGHelper _neighbourSelector;

		private int _generatedCount;
		private int[] _currentNeighbours;
		private int _currentDegree;

		public Tracking (int K,
		                 int rampPointPercent,
		                 int rampedMaxDegree)
		{
			_K = K;
			_generatedCount = 0;
			_generatedNeighboursCounter = new List<int> (K);

			_rampPoint = (K * rampPointPercent) / 100;
			_rampedMaxDegree = rampedMaxDegree;
			if (_rampedMaxDegree > K)
			{
				_rampedMaxDegree = K;
			}
			_rampedDegreeSelector = new CryptoRNGHelper ();
			_neighbourSelector = new CryptoRNGHelper ();

			_currentDegree = 1;
			_currentNeighbours = null;

			for (int n=0; n<K; n++)
			{
				_generatedNeighboursCounter.Add (0);
			}
		}

		public void Generate ()
		{
			if (_generatedCount < _rampPoint)
			{
				// Still degree 1
				++_generatedCount;
				_currentNeighbours = new int[_currentDegree];

				// Find an untouched neighbour
				int generatedNeighbour;
				do
				{
					generatedNeighbour = _neighbourSelector.Next (_K);
				} while (_generatedNeighboursCounter[generatedNeighbour] > 0);

				_generatedNeighboursCounter [generatedNeighbour]++;
				_currentNeighbours [0] = generatedNeighbour;
			}
			else
			{
				++_generatedCount;
				_currentDegree = _rampedDegreeSelector.Next (1, _rampedMaxDegree);
				List<int> generatedNeighboursList = new List<int> ();

				int generatedNeighbour;
				for (int n=0; n<_currentDegree; n++)
				{
					do
					{
						generatedNeighbour = _neighbourSelector.Next (_K);
						// If we already have that neighbour, then dont add
					} while (generatedNeighboursList.Contains (generatedNeighbour) == true);

					_generatedNeighboursCounter [generatedNeighbour]++;
					generatedNeighboursList.Add (generatedNeighbour);
				}

				_currentNeighbours = generatedNeighboursList.ToArray ();
			}
		}

		public int[] Neighbours ()
		{
			return _currentNeighbours;
		}

		public int Degree ()
		{
			return _currentDegree;
		}

		public int EstimateBlocks 
		{
			get
			{
				return _generatedNeighboursCounter.Count;
			}
		}
	}
}

