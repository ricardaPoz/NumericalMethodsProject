using NumericalMethods.Core.Approximation.Interfaces;

namespace NumericalMethods.Core.Approximation.InterpolationFunctions.Linear;
internal class LinearInterpolationFunction : IInterpolationFunction
{
	private readonly IEnumerable<IInterpolationNode> _interpolation_nodes;
	private readonly List<LinearFunction> _functions_of_interpolation_intervals;
	public LinearInterpolationFunction(IEnumerable<IInterpolationNode> interpolation_nodes)
	{
		_interpolation_nodes = interpolation_nodes;
		_functions_of_interpolation_intervals = CreateFunctionsOfInterpolationIntervals();
	}

	/// <summary>
	/// Создание функций участков интерполирования
	/// </summary>
	private List<LinearFunction> CreateFunctionsOfInterpolationIntervals()
	{
		List<LinearFunction> result = new List<LinearFunction>();
		int count_nodes = _interpolation_nodes.Count();
		for (int i = 1; i < count_nodes; i++)
		{
			IInterpolationNode range_starting_point = _interpolation_nodes.ElementAt(i - 1);
			IInterpolationNode range_endpoint = _interpolation_nodes.ElementAt(i);
			double x0 = range_starting_point.X;
			double fx0 = range_starting_point.Y;
			double x1 = range_endpoint.X;
			double fx1 = range_endpoint.Y;
			double a = fx0;
			double b = (fx1 - fx0) / (x1 - x0);
			LinearFunction current_linear_function = new LinearFunction(a, b, x0);
			result.Add(current_linear_function);
		}

		return result;
	}

	public double? Calculate(double argument)
	{
		int count_nodes = _interpolation_nodes.Count();
		LinearFunction? function_in_the_range_of_the_argument = null;
		for (int i = 0; i < count_nodes - 1; i++)
		{
			if (_interpolation_nodes.ElementAt(i).X <= argument && _interpolation_nodes.ElementAt(i + 1).X >= argument)
			{
				function_in_the_range_of_the_argument = _functions_of_interpolation_intervals.ElementAt(i);
				break;
			}
		}

		return function_in_the_range_of_the_argument is not null ?
			function_in_the_range_of_the_argument.Calculate(argument)
			: null;
	}
}
