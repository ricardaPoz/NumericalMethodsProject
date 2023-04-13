using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalMethods.Console
{
	public static class TestFactorial
	{
		private static readonly Dictionary<int, int> factorialCache = new Dictionary<int, int>() { {1, 1} };
		public static void Run()
		{
			var fact1 = CalculateFactorial(4);
			var fact2 = CalculateFactorial(5);
		}

		private static int CalculateFactorial(int value)
		{
			if (factorialCache.TryGetValue(value, out int cachedValue))
			{
				return cachedValue;
			}
			else
			{
				if (value < 2) return 1;
				int calculatedValue = value * CalculateFactorial(value - 1);
				factorialCache.Add(value, calculatedValue);
				return calculatedValue;
			}
		}
	}
}
