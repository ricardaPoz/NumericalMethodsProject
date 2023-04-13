using MathNet.Symbolics;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Shared;

namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.ModifiedNewton
{
    internal class ModifiedNewtonMethod:ISolvingMethod
    {
		IEnumerable<IEnumerable<double>> ISolvingMethod.SolveWithSteps(NonLinearEquationsSystem system, double eps, Dictionary<string, FloatingPoint> initialGuess)
		{
			Dictionary<string, FloatingPoint> sortedInitialGuess = initialGuess.OrderBy(x => x.Key).ToDictionary(el => el.Key, el => el.Value);


			VectorColumn lastVectorX = new VectorColumn(sortedInitialGuess.Select(el => el.Value.RealValue).ToArray());
			SquareMatrix minusInternalJacobiMatrix = -SquareMatrix.CreateJacobiMatrix(system.FunctionExpressions, initialGuess).Invert();
			double delta = double.MaxValue;
			List<List<double>> results = new List<List<double>>() { lastVectorX.ToList() };
			results.Last().Add(double.MinValue);
			while (delta > eps)
			{
				Dictionary<string, FloatingPoint> values = new();
				for (int i = 0; i < lastVectorX.Size; i++)
				{
					values.Add(sortedInitialGuess.ElementAt(i).Key, lastVectorX[i]);
				}

				VectorColumn? yVector = new VectorColumn(system.FunctionExpressions
					.Select(function => function.Evaluate(values))
					.Select(fp => fp.RealValue)
					.ToArray());

				VectorColumn newVectorX = lastVectorX + minusInternalJacobiMatrix * yVector;

				delta = (newVectorX - lastVectorX).GetNormM();

				List<double> result = newVectorX.ToList();
				result.Add(delta);
				results.Add(result);

				lastVectorX = newVectorX;
			}
			return results;
		}
	}
}
