using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Interfaces;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods;
using MathNet.Symbolics;

namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems;
public record class NonLinearEquationsSystemsSolver(ISolvingMethod SolvingMethod) : INonLinearEquationsSystemsSolver
{
	public IEnumerable<IEnumerable<double>> SolveWithSteps(NonLinearEquationsSystem system, double eps, Dictionary<string, FloatingPoint> initialGuess)
	{
		return SolvingMethod.SolveWithSteps(system, eps, initialGuess);
	}
}
