using MathNet.Symbolics;

using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Shared;

using System.Xml.Linq;

namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.Newton;
public class NewtonMethod : ISolvingMethod
{
	IEnumerable<IEnumerable<double>> ISolvingMethod.SolveWithSteps(NonLinearEquationsSystem system, double eps, Dictionary<string,FloatingPoint> initialGuess)
	{
		Dictionary<string, FloatingPoint> sortedInitialGuess = initialGuess.OrderBy(x => x.Key).ToDictionary(el => el.Key, el => el.Value);


		VectorColumn lastVectorX = new VectorColumn(sortedInitialGuess.Select(el => el.Value.RealValue).ToArray());
		double delta = double.MaxValue;//Вот тут рофлянка здесь делта максимальное значение double
		List<List<double>> results = new List<List<double>>() { lastVectorX.ToList()};
		results.Last().Add(double.MinValue);//А я добавляю минимальное значение первое ))))))
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

			VectorColumn newVectorX = lastVectorX + -SquareMatrix.CreateJacobiMatrix(system.FunctionExpressions, values).Invert() * yVector;

			delta = (newVectorX - lastVectorX).GetNormM();

			List<double> result = newVectorX.ToList();
			result.Add(delta);
			results.Add(result);

			lastVectorX = newVectorX;
		}
		return results;
	}
}