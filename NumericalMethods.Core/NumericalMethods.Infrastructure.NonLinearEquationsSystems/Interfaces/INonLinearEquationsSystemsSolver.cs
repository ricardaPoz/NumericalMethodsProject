using MathNet.Symbolics;
namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Interfaces;
public interface INonLinearEquationsSystemsSolver
{
	public IEnumerable<IEnumerable<double>> SolveWithSteps(NonLinearEquationsSystem system, double eps, Dictionary<string, FloatingPoint> initialGuess);
}
