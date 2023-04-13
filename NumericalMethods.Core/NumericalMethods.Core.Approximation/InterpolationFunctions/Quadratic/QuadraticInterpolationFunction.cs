using NumericalMethods.Core.Approximation.Interfaces;
namespace NumericalMethods.Core.Approximation.InterpolationFunctions.Quadratic;
internal class QuadraticInterpolationFunction : IInterpolationFunction
{
	private readonly IEnumerable<IInterpolationNode> _interpolation_nodes;
	private readonly List<(QuadraticFunction quadraticFunction, double startArgument)> _functions_of_interpolation_intervals;
	public QuadraticInterpolationFunction(IEnumerable<IInterpolationNode> _interpolation_nodes)
	{
		this._interpolation_nodes = _interpolation_nodes;
		this._functions_of_interpolation_intervals = CreateFunctionsOfInterpolationIntervals();
	}
	private List<(QuadraticFunction quadraticFunction, double startArgument)> CreateFunctionsOfInterpolationIntervals()
	{
		List<(QuadraticFunction quadraticFunction, double startArgument)> result = new List<(QuadraticFunction quadraticFunction, double startArgument)>();
		List<IInterpolationNode> pointsForConstructingQuadraticFunction = new List<IInterpolationNode>();
		int count_nodes = _interpolation_nodes.Count();
		for (int i = 0; i < count_nodes; i++)
		{
			if (i + 2 >= count_nodes)
				break;
			pointsForConstructingQuadraticFunction.AddRange(new List<IInterpolationNode>() { _interpolation_nodes.ElementAt(i), _interpolation_nodes.ElementAt(i + 1), _interpolation_nodes.ElementAt(i + 2) });
			if (i == 0)
				result.Add((MethodKrammer(CreateSystemOfEquations(pointsForConstructingQuadraticFunction)), pointsForConstructingQuadraticFunction.First().X));
			else
				result.Add((MethodKrammer(CreateSystemOfEquations(pointsForConstructingQuadraticFunction)), pointsForConstructingQuadraticFunction[1].X));
			pointsForConstructingQuadraticFunction.Clear();
		}
		return result;

	}
	public double? Calculate(double argument)
	{
		QuadraticFunction? quadraticFunction = null;
		for (int i = 0; i < _functions_of_interpolation_intervals.Count; i++)
		{
			if (argument > _interpolation_nodes.Last().X || argument < _interpolation_nodes.First().X)
				break;
			if (i == _functions_of_interpolation_intervals.Count - 1)
			{
				quadraticFunction = _functions_of_interpolation_intervals[i].quadraticFunction;
				break;
			}
			if (argument <= _functions_of_interpolation_intervals[i + 1].startArgument && argument >= _functions_of_interpolation_intervals[i].startArgument)
			{
				quadraticFunction = _functions_of_interpolation_intervals[i].quadraticFunction;
				break;
			}
		}
		return quadraticFunction is null ? null : quadraticFunction.Calculate(argument);
	}
	private List<(QuadraticFunction quadraticFunction, double y)> CreateSystemOfEquations(List<IInterpolationNode> pointsForConstructingQuadraticFunction)
	{
		List<(QuadraticFunction quadraticFunction, double y)> quadraticEquations = new List<(QuadraticFunction quadraticFunction, double y)>();
		foreach (IInterpolationNode node in pointsForConstructingQuadraticFunction)
		{
			quadraticEquations.Add((new QuadraticFunction(Math.Pow(node.X, 2.0), node.X, 1), node.Y));
		}

		return quadraticEquations;
	}
	private QuadraticFunction MethodKrammer(List<(QuadraticFunction quadraticFunction, double y)> quadraticEquations)
	{
		double determinant = FindDeterminantMatrix(FormingMatrix(quadraticEquations, 0));
		return new QuadraticFunction(
			FindDeterminantMatrix(FormingMatrix(quadraticEquations, 1)) / determinant,
			FindDeterminantMatrix(FormingMatrix(quadraticEquations, 2)) / determinant,
			FindDeterminantMatrix(FormingMatrix(quadraticEquations, 3)) / determinant
		);
	}
	private List<List<double>> SplittingMatrix(List<List<double>> matrix, (int i, int j) indexDeleteElement)
	{
		List<List<double>> splittingMatrix = new List<List<double>>();
		for (int i = 0; i < matrix.Count; i++)
		{
			if (indexDeleteElement.i == i) continue;
			splittingMatrix.Add(new List<double>());
			for (int j = 0; j < matrix[i].Count; j++)
			{
				if (indexDeleteElement.j == j) continue;
				splittingMatrix.Last().Add(matrix[i][j]);
			}
		}
		return splittingMatrix;
	}
	private double FindDeterminantMatrix(List<List<double>> matrixNDimensions)
	{
		if (matrixNDimensions.Count == 2)
			return matrixNDimensions[0][0] * matrixNDimensions[1][1] - matrixNDimensions[0][1] * matrixNDimensions[1][0];
		else
		{
			double[] determinantsMatrixNDimensions = new double[] { 1, 1, 1 };
			for (int j = 0; j < matrixNDimensions[0].Count; j++)
			{
				determinantsMatrixNDimensions[j] = FindDeterminantMatrix(SplittingMatrix(matrixNDimensions, (0, j)));
				if ((0 + 1 + j + 1) % 2 == 0)
					determinantsMatrixNDimensions[j] *= matrixNDimensions[0][j];
				else
					determinantsMatrixNDimensions[j] *= -matrixNDimensions[0][j];

			}
			return determinantsMatrixNDimensions.Sum();
		}
	}
	private List<List<double>> FormingMatrix(List<(QuadraticFunction quadraticFunction, double y)> quadraticEquations, int indexDeterminant)
	{
		List<List<double>> matrix = new List<List<double>>();
		quadraticEquations.ForEach(quadraticEquation =>
		{
			matrix.Add(new List<double>());
			switch (indexDeterminant)
			{
				case 0:
					{
						matrix.Last().Add(quadraticEquation.quadraticFunction.A);
						matrix.Last().Add(quadraticEquation.quadraticFunction.B);
						matrix.Last().Add(quadraticEquation.quadraticFunction.C);
						break;
					}
				case 1:
					{
						matrix.Last().Add(quadraticEquation.y);
						matrix.Last().Add(quadraticEquation.quadraticFunction.B);
						matrix.Last().Add(quadraticEquation.quadraticFunction.C);
						break;
					}
				case 2:
					{
						matrix.Last().Add(quadraticEquation.quadraticFunction.A);
						matrix.Last().Add(quadraticEquation.y);
						matrix.Last().Add(quadraticEquation.quadraticFunction.C);
						break;
					}
				case 3:
					{
						matrix.Last().Add(quadraticEquation.quadraticFunction.A);
						matrix.Last().Add(quadraticEquation.quadraticFunction.B);
						matrix.Last().Add(quadraticEquation.y);
						break;
					}
			}
		});
		return matrix;
	}
}

