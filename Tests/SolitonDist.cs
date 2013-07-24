using System;
using NUnit.Framework;
using LubyTransform.Distributions;

namespace Tests
{
	[TestFixture()]
	public class SolitonDist
	{
		[Test()]
		public void Robust()
		{
			int num = 20;
			Soliton s = new Soliton (num, 2, 0.1);
			Random r = new Random ();

			Console.WriteLine ("Starting... ");
			for (int i=0; i<num; i++)
			{
				Console.WriteLine ("V{0}: {1}", i, s.Robust (r.Next()));
			}
		}
	}
}

