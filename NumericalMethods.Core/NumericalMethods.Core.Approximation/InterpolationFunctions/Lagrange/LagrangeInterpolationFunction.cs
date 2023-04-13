using NumericalMethods.Core.Approximation.Interfaces;

namespace NumericalMethods.Core.Approximation.InterpolationFunctions.Lagrange;
public class LagrangeInterpolationFunction : IInterpolationFunction
{
	private readonly IEnumerable<IInterpolationNode> _interpolation_nodes;
	public LagrangeInterpolationFunction(IEnumerable<IInterpolationNode> interpolation_nodes)
	{
		_interpolation_nodes = interpolation_nodes;
	}
	public double? Calculate(double argument)
	{
		double result = 0.0;
		int count_nodes = _interpolation_nodes.Count();
		for (int i = 0; i < count_nodes; i++)
		{
			double yi = _interpolation_nodes.ElementAt(i).Y;
			result += (yi * CalculateBasisPolynomial(argument, i));
		}

		return result;
	}

	/// <summary>
	/// Вычисление базисного полинома l(x)
	/// </summary>
	private double CalculateBasisPolynomial(double x, int i)
	{
		int count_nodes = _interpolation_nodes.Count();
		double xi = _interpolation_nodes.ElementAt(i).X;
		double multiply = 1;
		for (int j = 0; j < count_nodes; j++)
		{
			if (j == i) continue;
			double xj = _interpolation_nodes.ElementAt(j).X;
			multiply *= (x - xj) / (xi - xj);
		}

		return multiply;
	}
}
