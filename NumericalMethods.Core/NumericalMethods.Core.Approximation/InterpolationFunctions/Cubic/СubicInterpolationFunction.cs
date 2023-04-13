using NumericalMethods.Core.Approximation.Interfaces;

namespace NumericalMethods.Core.Approximation.InterpolationFunctions.Cubic;
public class СubicInterpolationFunction : IInterpolationFunction
{
	private readonly IEnumerable<IInterpolationNode> _interpolation_nodes;
	private readonly List<CubicFunction> _functions_of_interpolation_intervals;

	public СubicInterpolationFunction(IEnumerable<IInterpolationNode> interpolation_nodes)
	{
		_interpolation_nodes = interpolation_nodes;
		_functions_of_interpolation_intervals = CreateFunctionsOfInterpolationIntervals();
	}

	private List<CubicFunction> CreateFunctionsOfInterpolationIntervals()
	{
		int count_splines = _interpolation_nodes.Count() - 1;
		double[] a = new double[count_splines];
		double[] b = new double[count_splines];
		double[] c = new double[count_splines];
		double[] d = new double[count_splines];
		double[] h = new double[count_splines];

		for (int i = 0; i < count_splines; i++)
		{
			a[i] = _interpolation_nodes.ElementAt(i).Y;
			h[i] = _interpolation_nodes.ElementAt(i + 1).X - _interpolation_nodes.ElementAt(i).X;
		}

		List<double> c_left = new List<double>(count_splines - 1);
		List<double> c_center = new List<double>(count_splines);
		List<double> c_right = new List<double>(count_splines - 1);
		List<double> free_members = new List<double>(count_splines);

		c_center.Add(1);
		c_right.Add(0);
		free_members.Add(0);

		for (int i = 1; i < count_splines; i++)
		{
			double left = h[i - 1];
			double right = h[i];
			double center = 2.0 * (h[i - 1] + h[i]);
			double free_member = 3 * (((_interpolation_nodes.ElementAt(i + 1).Y - _interpolation_nodes.ElementAt(i).Y) / h[i])
				- ((_interpolation_nodes.ElementAt(i).Y - _interpolation_nodes.ElementAt(i - 1).Y) / h[i - 1]));
			c_left.Add(left);
			if (i != count_splines - 1) c_right.Add(right);
			c_center.Add(center);
			free_members.Add(free_member);
		}

		SweepMethod sweep_method = new SweepMethod(c_left.ToArray(), c_center.ToArray(), c_right.ToArray(), free_members.ToArray());
		c = sweep_method.Calculate().ToArray();

		for (int i = 0, j = 1; i < c.Length - 1 && j < count_splines; i++, j++)
		{
			b[i] = ((_interpolation_nodes.ElementAt(j).Y - _interpolation_nodes.ElementAt(j - 1).Y) / h[i])
				- (h[i] / 3 * ((2 * c[i]) + c[i + 1]));
		}

		for (int i = 0; i < c.Length - 1; i++)
		{
			d[i] = (c[i + 1] - c[i]) / (3 * h[i]);
		}

		int n = count_splines - 1;

		d[n] = -c[n] / (3 * h[n]);
		b[n] = ((_interpolation_nodes.ElementAt(n + 1).Y - _interpolation_nodes.ElementAt(n).Y) / h[n])
			- (2.0 / 3.0 * h[n] * c[n]);

		List<CubicFunction> result = new List<CubicFunction>();

		for (int i = 0; i < count_splines; i++)
		{
			result.Add(new CubicFunction(a[i], b[i], c[i], d[i], _interpolation_nodes.ElementAt(i).X));
		}

		return result;
	}

	public double? Calculate(double argument)
	{
		var count_nodes = _interpolation_nodes.Count();
		CubicFunction? function_in_the_range_of_the_argument = null;
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
