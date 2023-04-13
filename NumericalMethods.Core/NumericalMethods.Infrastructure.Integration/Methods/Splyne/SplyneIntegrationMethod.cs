using NumericalMethods.Infrastructure.Integration.Methods.Trapezoid;
using MathNet.Symbolics;
using NumericalMethods.Infrastructure.Integration.Shared;

namespace NumericalMethods.Infrastructure.Integration.Methods.Splyne
{
	public record class SplyneIntegrationMethod : IIntegrationMethodWithConstantStep
	{
		public double Integrate(string function, double start, double end, double step)
		{
			SymbolicExpression func = SymbolicExpression.Parse(function);
			double inaccuracyTrapezoidMethod = 0;//Погрешность метода трапеции
			double trapezoidIntegral = new TrapezoidIntegrationMethod().Integrate(function, start, end, step); // Первая часть функции Сплайна
			for (double x = start + step; x < end; x += step)
				inaccuracyTrapezoidMethod += Math.Pow(step, 3) * func.Derivative(x, 2);
			return trapezoidIntegral - (inaccuracyTrapezoidMethod / 12);
		}
	}
}
