using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem;
public record class CauchyProblemSolverWithOneStepMethod(IOneStepMethod Method, string function) : ICauchyProblemSolver
{
    public ResultTable Calculate(double b, double h, (double x, Dictionary<string, double> ys) initialGuess)
    {
        Dictionary<string, double> sortedYs = initialGuess.ys
                .OrderBy(pair => pair.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        KeyValuePair<string, double> maxOrderY = sortedYs.Last();
        var order = int.Parse(maxOrderY.Key[1..]);
        Dictionary<string, string> functions = new Dictionary<string, string>();
        for (int i = 0; i < order; i++)
        {
            functions.Add(sortedYs.ElementAt(i).Key, sortedYs.ElementAt(i + 1).Key);
        }
        functions.Add(maxOrderY.Key, function);
        ResultTable result = new ResultTable(order);
        result.Add(initialGuess.x, sortedYs);
        for (double current_x = initialGuess.x + h; Math.Round(current_x, 7) <= Math.Round(b, 7); current_x += h)
        {
            Dictionary<string, double> ys = new Dictionary<string, double>();
            Dictionary<string, FloatingPoint> conditions = new Dictionary<string, FloatingPoint>() { { "x", current_x - h } };
            foreach (var yName in initialGuess.ys.Keys)
            {
                conditions.Add(yName, result[yName].Last().yi);
            }
            foreach (KeyValuePair<string, double> y in initialGuess.ys)
            {
                var newValueY = Method.Calculate(SymbolicExpression.Parse(functions[y.Key]), h, current_x - h, (y.Key, conditions[y.Key].RealValue), conditions);
                ys.Add(y.Key, newValueY);
            }
            result.Add(current_x, ys);
        }
        return result;
    }
}
