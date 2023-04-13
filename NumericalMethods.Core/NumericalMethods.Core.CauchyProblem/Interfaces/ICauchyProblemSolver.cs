namespace NumericalMethods.Core.CauchyProblem.Interfaces;
public interface ICauchyProblemSolver
{
    ResultTable Calculate(double b, double h, (double x, Dictionary<string, double> ys) initialGuess);
}
