using MathNet.Symbolics;

using NumericalMethods.Infrastructure.Integration.Shared;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Interfaces;

namespace NumericalMethods.Infrastructure.Integration.Methods.Chebyshev;
public class ChebyshevIntegrationMethod : IIntegrationMethodWithVariableStep
{
    /// <summary> Интегрирование отрезка функции </summary>
    /// <param name="function"> Подинтегральная функция </param>
    /// <param name="start"> Начало отрезка интегрирования </param>
    /// <param name="end"> Конец отрезка интегрирования </param>
    /// <param name="count_nodes"> Количество разбиений отрезка интегрирования </param>
    /// <returns></returns>
    public double Integrate(string function, double start, double end, int count_nodes)
    {
        SymbolicExpression functionExpression = SymbolicExpression.Parse(function);
        IEnumerable<double> tRoots = GetRootsForCountNodes(count_nodes);
        IEnumerable<double> xCoefs = tRoots.Select(tRoot => GetRealX(start, end, tRoot));
        IEnumerable<double> aCoefs = Enumerable.Repeat(2.0 / count_nodes, count_nodes);
		IEnumerable<double> aCoefsMultiplyedXs = aCoefs.Zip(aCoefs, (aCoef, xCoef) => aCoef * functionExpression.EvaluateX(xCoef));
        return (end - start) / 2 * aCoefsMultiplyedXs.Sum();
    }

    /// <summary> Функция зависимости x от t </summary>
    /// <param name="a"> Начало отрезка интегрирования </param>
    /// <param name="b"> Конеч отрезка интегрирования </param>
    /// <param name="t"> Значение коэффициента t </param>
    private static double GetRealX(double a, double b, double t)
    {
        return (a + b) / 2 + (b - a) / 2 * t;
    }

    /// <summary> Возвращает значения корней Чебышева для определенного количества разбиений отрезка интегрирования </summary>
    /// <param name="countNodes"> Количество разбиений </param>
    /// <exception cref="NotImplementedException"></exception>
    private static IEnumerable<double> GetRootsForCountNodes(int countNodes)
	{
        return countNodes switch
        {
            3 => new double[3] { 0.707107, 0, -0.707107 },
            4 => new double[4] { 0.794654, 0.187592, -0.794654, -0.187592 },
            5 => new double[5] { 0.832498, 0.374541, 0, -0.832498, -0.374541 },
            6 => new double[6] { 0.866247, 0.422519, 0.266635, -0.866247, -0.422519, -0.266635 },
            7 => new double[7] { 0.883862, 0.529657, 0.323912, 0, -0.883862, -0.529657, -0.323912 },
            9 => new double[9] { 0.911589, 0.601019, 0.528762, 0.167906, 0, -0.911589, -0.601019, -0.528762, -0.167906 },
            _ => throw new NotImplementedException($"Для размерности {countNodes} метод не реализован")
        };
	}

    /// <summary> Создает задачу поиска начального приближения для вычисления значений корней Чебышева </summary>
    private Task<IEnumerable<double>?> CreateRootFinder(List<string> expressions, CancellationToken token)
	{
        return Task.Run(() =>
        {
			List<string> expressionsCopy = expressions.ToList();
			NonLinearEquationsSystem snu = new NonLinearEquationsSystem(expressionsCopy);
            INonLinearEquationsSystemsSolver solver = new NonLinearEquationsSolverBuilder().Build(SolvingMethods.Newton);
            bool success = false;
            Random random = new Random();
            IEnumerable<double> randomInitial = new double[snu.FunctionExpressions.Count()].Select(value => random.Next(-1, 2) + random.NextDouble());
            IEnumerable<double> findedRoots = null;
            while (success is false)
			{
                try
				{
                    if (token.IsCancellationRequested) break;
                    //findedRoots = solver.Solve(snu, 0.001d, randomInitial);
                    if (findedRoots.Any(root => double.IsNaN(root) || root is < -1 or > 1))
                    {
                        randomInitial = randomInitial.Select(value => random.Next(-1, 2) + random.NextDouble());
                        continue;
                    }

                    success = true;
                } catch { randomInitial = randomInitial.Select(value => random.Next(-1, 2) + random.NextDouble()); }
            }

            return findedRoots;
        }, token);
	}
    
    /// <summary> Создает определенное количество задач поиска начального приближения для вычисления значений корней Чебышева </summary>
    private List<Task<IEnumerable<double>>> CreateFinders(List<string> expressions, CancellationToken token, int count)
	{
        List<Task<IEnumerable<double>>> finders = new List<Task<IEnumerable<double>>>(); 
		for (int i = 0; i < count; i++)
		{
            finders.Add(CreateRootFinder(expressions, token));
		}
        return finders;
	}
}
