using MathNet.Symbolics;
using NumericalMethods.Infrastructure.Integration.Shared;

namespace NumericalMethods.Infrastructure.Integration.Methods.Parabolic;
public class ParabolicIntegrationMethod : IIntegrationMethodWithConstantStep
{
	public double Integrate(string function, double start, double end, double step)
	{
		SymbolicExpression func = SymbolicExpression.Parse(function);
		double valueEvenFunction = 0, valueOddFunction = 0;
		int i = 0;
		for (double x = start + step; x < end - step; x += step)
			if (++i % 2 == 0)
				valueEvenFunction += func.EvaluateX(x);
			else
				valueOddFunction += func.EvaluateX(x);
		return step / 3 * (func.EvaluateX(start) + (4 * valueOddFunction) + (2 * valueEvenFunction) + func.EvaluateX(end));
	}
}
