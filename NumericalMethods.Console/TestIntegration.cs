using MathNet.Symbolics;

using NumericalMethods.Infrastructure.Integration;

using System;

namespace NumericalMethods.Console
{
	public static class TestIntegration
	{
		public static void Run()
		{
			List<string> functions = new()
			{
				"2*x1^2 - x1*x2 - 5*x1 + 1",
				"x1 + 3*lg(x1) - x2^2"
			};

			List<SymbolicExpression> functionExpression = functions.Select(function => SymbolicExpression.Parse(function)).ToList();

			Dictionary<string, FloatingPoint> values = new()
			{
				{ "x1", 0 },
				{ "x2", 1 }
			};

			var res = SymbolicExpression.Parse("2*x1^2 - x1*x2 - 5*x1 + 1 - x1 - 3*lg(x1) + x2^2");

			double eps = 0.001d;

			double step = 0.001;
			int countNodesGauss = 9;
			int countNodesChebyshev = 9;
			int countNodesMonteCarlo = 10000;

			string function = "x^2";
			double start = 1;
			double end = 2;

			var resultR = new IntegrationBuilder()
				.Build(function, IntegrationMethodsWithConstantStep.Rectangle)
				.Integrate(start, end, step);

			var resultT = new IntegrationBuilder()
				.Build(function, IntegrationMethodsWithConstantStep.Trapeze)
				.Integrate(start, end, step);

			var resultS = new IntegrationBuilder()
				.Build(function, IntegrationMethodsWithConstantStep.Spline)
				.Integrate(start, end, step);

			var resultP = new IntegrationBuilder()
				.Build(function, IntegrationMethodsWithConstantStep.Parabolic)
				.Integrate(start, end, step);

			var resultG = new IntegrationBuilder()
				.Build(function, IntegrationMethodsWithVariableStep.Gauss)
				.Integrate(start, end, countNodesChebyshev);

			var resultC = new IntegrationBuilder()
				.Build(function, IntegrationMethodsWithVariableStep.Chebyshev)
				.Integrate(start, end, countNodesChebyshev);

			System.Console.WriteLine($"Rectangle:  {resultR}");
			System.Console.WriteLine($"Trapeze:    {resultT}");
			System.Console.WriteLine($"Parabolic:  {resultP}");
			System.Console.WriteLine($"Spline:     {resultS}");
			System.Console.WriteLine($"Gauss:      {resultG}");
			System.Console.WriteLine($"Chebyshev:  {resultC}");
		}
	}
}
