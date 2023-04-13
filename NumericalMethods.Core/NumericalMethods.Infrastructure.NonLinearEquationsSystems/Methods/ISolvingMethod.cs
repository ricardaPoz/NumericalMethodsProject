using MathNet.Symbolics;
namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods;
public interface ISolvingMethod
{
	public IEnumerable<IEnumerable<double>> SolveWithSteps(NonLinearEquationsSystem system, double eps, Dictionary<string,FloatingPoint> initialGuess);
}
