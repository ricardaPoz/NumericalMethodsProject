using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
public class AdamsMethod : IMultiStepMethod
{
    public ResultTable Calculate(List<SymbolicExpression> functions, double b, double h, ResultTable initialResult)
    {
        if (initialResult.CountRow < 4) throw new ArgumentException($"{nameof(initialResult)} count row less then 4");

        for (double current_x = initialResult["x"].Last().x + h; current_x <= b; current_x += h)
        {
            Dictionary<string, double> new_ys = new Dictionary<string, double>();
            List<Dictionary<string, FloatingPoint>> last_four_rows = initialResult.GetRows(^4..)
                .Select(row => row.Select(pair => new KeyValuePair<string, FloatingPoint>(pair.Key, pair.Value))
                    .ToDictionary(pair => pair.Key, pair => pair.Value))
                .ToList();
            for (int i = 0; i < functions.Count; i++)
            {
                string current_y_name = last_four_rows.Last().ElementAt(i + 1).Key;
                double current_y_value = last_four_rows.Last()[current_y_name].RealValue;
                double fi = functions[i].Evaluate(last_four_rows.Last()).RealValue;
                double fi_minus_one = functions[i].Evaluate(last_four_rows.SkipLast(1).Last()).RealValue;
                double fi_minus_two = functions[i].Evaluate(last_four_rows.SkipLast(2).Last()).RealValue;
                double fi_minus_three = functions[i].Evaluate(last_four_rows.SkipLast(3).Last()).RealValue;
                double delta_f = fi - fi_minus_one;
                double delta_f_two = fi - 2 * fi_minus_one + fi_minus_two;
                double delta_f_three = fi - 3 * fi_minus_one + 3 * fi_minus_two - fi_minus_three;
                double new_y_value = current_y_value + fi * h + Math.Pow(h, 2) / 2.0 * delta_f + 5 * Math.Pow(h, 3) / 12.0 * delta_f_two + 3 * Math.Pow(h, 4) / 8.0 * delta_f_three;
                new_ys.Add(current_y_name, new_y_value);
            }
            initialResult.Add(current_x, new_ys);
        }
        return initialResult;
    }
}
