using NumericalMethods.Core.Approximation.Interfaces;
using NumericalMethods.Core.Approximation.InterpolationFunctions.Cubic;
using NumericalMethods.Core.Approximation.InterpolationFunctions.Lagrange;
using NumericalMethods.Core.Approximation.InterpolationFunctions.Linear;
using NumericalMethods.Core.Approximation.InterpolationFunctions.Quadratic;

namespace NumericalMethods.Core.Approximation;
public static class InterpolationBuilder
{
	public static IInterpolationFunction? Build(IEnumerable<IInterpolationNode> interpolation_nodes, InterpolationFunctionType function_type)
	{
		return function_type switch
		{
			InterpolationFunctionType.Linear => new LinearInterpolationFunction(interpolation_nodes),
			InterpolationFunctionType.Quadratic => new QuadraticInterpolationFunction(interpolation_nodes),
			InterpolationFunctionType.Cubic => new СubicInterpolationFunction(interpolation_nodes),
			InterpolationFunctionType.LagrangePolynomials => new LagrangeInterpolationFunction(interpolation_nodes),
			_ => null
		};
	}
}
