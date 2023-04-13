using MathNet.Symbolics;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Shared;

namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.Secant
{
    internal class SecantMethod:ISolvingMethod
    {
		IEnumerable<IEnumerable<double>> ISolvingMethod.SolveWithSteps(NonLinearEquationsSystem system, double eps, Dictionary<string, FloatingPoint> initialGuess)
		{
			//Здесь я избавился от переменной values пример смотреть с методом ньютона
			Dictionary<string, FloatingPoint> sortedInitialGuess = initialGuess.OrderBy(x => x.Key).ToDictionary(el=>el.Key,el=>el.Value);

			VectorColumn lastVectorX = new VectorColumn(sortedInitialGuess.Select(el => el.Value.RealValue).ToArray());

			SquareMatrix lastA = SquareMatrix.CreateJacobiMatrix(system.FunctionExpressions, sortedInitialGuess);

			//Добавляем начальное приблежение и указываем что норма равна double.MinValue
			List<List<double>> results = new List<List<double>>() { lastVectorX.ToList() };
			results.Last().Add(double.MinValue);

			//Расчёт следующих значение x-ов
			VectorColumn? YVector = new VectorColumn(system.FunctionExpressions
				.Select(function => function.Evaluate(sortedInitialGuess))
				.Select(fp => fp.RealValue)
				.ToArray());
			VectorColumn s = -lastA.Invert() * YVector;
			VectorColumn newVectorX = lastVectorX + s;
			double delta = s.GetNormM();

			//Добавляем первый полученный вектор
			List<double> result = newVectorX.ToList();
			result.Add(delta);
			results.Add(result);

			lastVectorX = newVectorX;

			VectorColumn? newYVector;
			VectorColumn deltaYVector;
			SquareMatrix newA;
			while (delta > eps)
			{
				for (int i = 0; i < lastVectorX.Size; i++)
				{
					sortedInitialGuess[sortedInitialGuess.ElementAt(i).Key] = lastVectorX[i];
				}

				newYVector = new VectorColumn(system.FunctionExpressions
					.Select(function => function.Evaluate(sortedInitialGuess))
					.Select(fp => fp.RealValue)
					.ToArray());
				deltaYVector = newYVector - YVector;
				newA = lastA + ((deltaYVector - lastA * s) * s.Transposition()/(s.Transposition() * s).Value);
				s = -newA.Invert() * newYVector;
				newVectorX = lastVectorX + s;
				delta = s.GetNormM();

				//Эти три строчки связаны с добавлением разности(так была бы 1)
				result = newVectorX.ToList();
				result.Add(delta);
				results.Add(result);

				lastA = newA;
				YVector = newYVector;
				lastVectorX = newVectorX;
			}
			return results;
		}
	}
}
