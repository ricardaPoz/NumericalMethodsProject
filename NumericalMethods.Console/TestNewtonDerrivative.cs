using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Symbolics;

namespace NumericalMethods.Console
{
	public static class TestNewtonDerrivative
	{
		private static readonly Dictionary<int, int> factorialCache = new Dictionary<int, int>() { { 1, 1 } };
		public static void Run()
		{
			string function = GetNewtonPolynomial(4);
			string? derFunc = GetNewtonPolynomialDerrivative(1, 10);
		}

		private static string GetNewtonPolynomial(int numberOfMembers)
		{
			if (numberOfMembers == 1)
			{
				return "f";
			}
			else if (numberOfMembers == 2)
			{
				return "f + t*y";
			}
			else
			{
				StringBuilder result = new StringBuilder("f + t*y1");
				for (int i = 3; i <= numberOfMembers; i++)
				{
					int n = i - 1;
					int factorial = CalculateFactorial(n);
					List<string> bracketsList = new List<string>();
					for (int j = 0; j < n - 1; j++)
					{
						bracketsList.Add($"(t - {j + 1})");
					}

					string bracketsString = string.Join(" * ", bracketsList);
					string member = $"t * {bracketsString}/{factorial}*y{n}^{n}";
					result.Append($" + {member}");
				}

				return result.ToString();
			}
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

		private static string? GetNewtonPolynomialDerrivative(int derrivativeDegree, int numberOfMembers)
		{
			if (derrivativeDegree < 1 || derrivativeDegree + 1 > numberOfMembers) return null;
			string newtonPolynomial = GetNewtonPolynomial(numberOfMembers);
			SymbolicExpression result = SymbolicExpression.Parse(newtonPolynomial).Expand();
			for (int i = 0; i < derrivativeDegree; i++)
			{
				result = result.Differentiate("t");
			}
			return result.ToString();
		}
	}
}
