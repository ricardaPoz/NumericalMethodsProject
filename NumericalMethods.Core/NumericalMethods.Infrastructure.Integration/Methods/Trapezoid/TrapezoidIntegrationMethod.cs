using MathNet.Symbolics;
using NumericalMethods.Infrastructure.Integration.Shared;

namespace NumericalMethods.Infrastructure.Integration.Methods.Trapezoid;
public class TrapezoidIntegrationMethod : IIntegrationMethodWithConstantStep
{
	public double Integrate(string function, double start, double end, double step)
	{
		SymbolicExpression func = SymbolicExpression.Parse(function);
		double sumElement = 0;
		for (double x = start + step; x < end; x += step)
			sumElement += step * (func.EvaluateX(x - step) + func.EvaluateX(x));
		return sumElement / 2;
	}
}