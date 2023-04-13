using MathNet.Symbolics;
using NumericalMethods.Infrastructure.Integration.Shared;

namespace NumericalMethods.Infrastructure.Integration.Methods.Rectangle;
internal class RectangleIntegrationMethod : IIntegrationMethodWithConstantStep
{
	public double Integrate(string function, double start, double end, double step)
	{
		SymbolicExpression func = SymbolicExpression.Parse(function);
		double resulted_sum = 0;
		for (double xi = start + (step / 2); xi < end; xi += step)
			resulted_sum += step * func.EvaluateX(xi);
		return resulted_sum;
	}
}