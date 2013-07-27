using System;

namespace LubyTransform.Distributions
{
	public class Soliton : IDistribution
	{
		/// <summary>
		/// Number of blocks to be encoded.
		/// </summary>
		private int _k;

		/// <summary>
		/// Admissible failure probability.
		/// </summary>
		private double _delta;

		/// <summary>
		/// Parameter used for the Tau() function.
		/// </summary>
		private double _R;

		/// <summary>
		/// Normalization factor for the robust soliton distribution.
		/// </summary>
		private double _beta; 

		private Random _rand;

		/// <summary>
		/// Initializes a new instance of the <see cref="LubyTransform.Soliton"/> class.
		/// </summary>
		/// <param name="k">
		/// Number of blocks to be encoded.
		/// </param>
		/// <param name="c">
		/// Constant
		/// </param>
		/// <param name="delta">
		/// Admissible failure probability.
		/// </param>
		public Soliton (int k, double c, double delta) 
		{
			int i;
			_rand = new Random ((int)DateTime.Now.Ticks);

			if (k > 0 && c > 0 && delta >= 0 && delta <= 1)
			{
				_k = k;
				_delta = delta;
			}
			else
			{
				throw new ArgumentException ();
			}

			_R = c * Math.Log (k / delta) * Math.Sqrt (k);

			for (_beta=0,i=1; i<=k; i++)
			{
				try
				{
					_beta += Rho (i) + Tau (i);
				}
				catch (Exception e)
				{
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
				}
			}
		}

		/// <summary>
		/// Ideal soliton distribution.
		/// </summary>
		/// <param name="i">
		/// Argument for the probability mass function.
		/// </param>
		/// <returns>
		/// Value (respective to input) of the probability mass function.
		/// </returns>
		private double Rho(int i) 
		{ 
			if (i < 1 || i > _k)
			{
				throw new ArgumentOutOfRangeException ();
			}

			if(i == 1){
				return(1.0 / _k);
			}
			else
			{
				return(1.0 / (i * (i - 1.0)));
			}
		}

		/// <summary>
		/// Extra set of values to the elements of mass function of the ideal soliton distribution.
		/// </summary>
		/// <param name="i">
		/// Argument for the probability mass function.
		/// </param>
		/// <returns>
		/// Value (respective to input) of the probability mass function.
		/// </returns>
		private double Tau(int i) 
		{
			if(i < 1 || i > _k)
			{
				throw new ArgumentOutOfRangeException ();
			}

			int kR = (int) Math.Round (_k / _R);

			if(i < kR)
			{
				return(_R / (i * _k));
			}
			else
			{
				if (i > kR)
				{
					return 0;
				}
				else
				{
					return _R * Math.Log (_R / _delta) / _k;
				}
			}
		}

		/// <summary>
		/// Robust soliton distribution.
		/// </summary>
		/// <param name="i">
		/// Argument for the probability mass function.
		/// </param>
		/// <returns>
		/// Value (respective to input) of the probability mass function.
		/// </returns>
		private double Mu(int i) 
		{
			return (Rho(i) + Tau(i)) / _beta;
		}

		/// <summary>
		/// Samples the robust soliton distribution.
		/// </summary>
		/// <param name="seed">Seed.</param>
		/// <returns>
		/// Random degree number, according to the robust soliton probability distribution.
		/// </returns>
		public int Degree (int seed) 
		{
			double r = _rand.NextDouble ();
			double sum = 0;
			int d = 0;

			while(sum < r)
			{
				sum += Mu (++d);
			}

			return d;
		}

		/// <summary>
		/// Calculates the number of encoded packets required at the receiving end to ensure 
		/// that the decoding can run to completion, with probability at least 1-delta.
		/// </summary>
		/// <value> 
		/// The number of encoded packets required at the receiving end to ensure 
		/// that the decoding can run to completion, with probability at least 1-delta.
		/// </value>
		public int EstimateBlocks 
		{
			get
			{
				return (int)(_k * _beta);
			}
		}
	}
}

