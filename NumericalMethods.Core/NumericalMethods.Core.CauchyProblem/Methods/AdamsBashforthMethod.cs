using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
public class AdamsBashforthMethod : IMultiStepMethod
{
    public ResultTable Calculate(List<SymbolicExpression> functions, double b, double h, ResultTable initialResult)
    {
        if (initialResult.CountRow < 1) throw new ArgumentException($"{nameof(initialResult)} count row less then 1");

        Func<Dictionary<string, double>, Dictionary<string, FloatingPoint>> convert_with_floating_point =
            (row) => row
                .Select(pair => new KeyValuePair<string, FloatingPoint>(pair.Key, pair.Value))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        double current_x = initialResult["x"].Last().x + h;
        if (initialResult.CountRow == 1)
        {
            Dictionary<string, double> new_ys = new Dictionary<string, double>();
            Dictionary<string, FloatingPoint> first_row = convert_with_floating_point(initialResult.GetRow(0));
            for (int i = 0; i < functions.Count; i++)
            {
                string current_y_name = first_row.ElementAt(i + 1).Key;
                double current_y_value = first_row[current_y_name].RealValue;
                double new_y_value = current_y_value + h * functions[i].Evaluate(first_row).RealValue;
                new_ys.Add(current_y_name, new_y_value);
            }
            initialResult.Add(current_x, new_ys);
            current_x += h;
            if (current_x > b) return initialResult;
        }
        if (initialResult.CountRow == 2)
        {
            Dictionary<string, double> new_ys = new Dictionary<string, double>();
            Dictionary<string, FloatingPoint> first_row = convert_with_floating_point(initialResult.GetRow(0));
            Dictionary<string, FloatingPoint> second_row = convert_with_floating_point(initialResult.GetRow(1));
            for (int i = 0; i < functions.Count; i++)
            {
                string current_y_name = second_row.ElementAt(i + 1).Key;
                double current_y_value = second_row[current_y_name].RealValue;
                double new_y_value = current_y_value + h * (3.0 / 2.0 * functions[i].Evaluate(second_row).RealValue
                    - 1.0 / 2.0 * functions[i].Evaluate(first_row).RealValue);
                new_ys.Add(current_y_name, new_y_value);
            }
            initialResult.Add(current_x, new_ys);
            current_x += h;
            if (current_x > b) return initialResult;
        }
        if (initialResult.CountRow == 3)
        {
            Dictionary<string, double> new_ys = new Dictionary<string, double>();
            Dictionary<string, FloatingPoint> first_row = convert_with_floating_point(initialResult.GetRow(0));
            Dictionary<string, FloatingPoint> second_row = convert_with_floating_point(initialResult.GetRow(1));
            Dictionary<string, FloatingPoint> third_row = convert_with_floating_point(initialResult.GetRow(2));
            for (int i = 0; i < functions.Count; i++)
            {
                string current_y_name = third_row.ElementAt(i + 1).Key;
                double current_y_value = third_row[current_y_name].RealValue;
                double new_y_value = current_y_value + h * (23.0 / 12.0 * functions[i].Evaluate(third_row).RealValue
                    - 4.0 / 3.0 * functions[i].Evaluate(second_row).RealValue
                    + 5.0 / 12.0 * functions[i].Evaluate(first_row).RealValue);
                new_ys.Add(current_y_name, new_y_value);
            }
            initialResult.Add(current_x, new_ys);
            current_x += h;
            if (current_x > b) return initialResult;
        }
        if (initialResult.CountRow == 4)
        {
            Dictionary<string, double> new_ys = new Dictionary<string, double>();
            Dictionary<string, FloatingPoint> first_row = convert_with_floating_point(initialResult.GetRow(0));
            Dictionary<string, FloatingPoint> second_row = convert_with_floating_point(initialResult.GetRow(1));
            Dictionary<string, FloatingPoint> third_row = convert_with_floating_point(initialResult.GetRow(2));
            Dictionary<string, FloatingPoint> four_row = convert_with_floating_point(initialResult.GetRow(3));
            for (int i = 0; i < functions.Count; i++)
            {
                string current_y_name = four_row.ElementAt(i + 1).Key;
                double current_y_value = four_row[current_y_name].RealValue;
                double new_y_value = current_y_value + h * (55.0 / 24.0 * functions[i].Evaluate(four_row).RealValue
                    - 59.0 / 24.0 * functions[i].Evaluate(third_row).RealValue
                    + 37.0 / 24.0 * functions[i].Evaluate(second_row).RealValue
                    - 3.0 / 8.0 * functions[i].Evaluate(first_row).RealValue);
                new_ys.Add(current_y_name, new_y_value);
            }
            initialResult.Add(current_x, new_ys);
            current_x += h;
            if (current_x > b) return initialResult;
        }


        for (; current_x <= b; current_x += h)
        {
            Dictionary<string, double> new_ys = new Dictionary<string, double>();
            Dictionary<string, FloatingPoint> last_five_row = convert_with_floating_point(initialResult.GetRow(initialResult.CountRow - 5));
            Dictionary<string, FloatingPoint> last_four_row = convert_with_floating_point(initialResult.GetRow(initialResult.CountRow - 4));
            Dictionary<string, FloatingPoint> last_third_row = convert_with_floating_point(initialResult.GetRow(initialResult.CountRow - 3));
            Dictionary<string, FloatingPoint> last_second_row = convert_with_floating_point(initialResult.GetRow(initialResult.CountRow - 2));
            Dictionary<string, FloatingPoint> last_first_row = convert_with_floating_point(initialResult.GetRow(initialResult.CountRow - 1));
            for (int i = 0; i < functions.Count; i++)
            {
                string current_y_name = last_first_row.ElementAt(i + 1).Key;
                double current_y_value = last_first_row[current_y_name].RealValue;
                 double new_y_value = current_y_value + h * (1901.0 / 720.0 * functions[i].Evaluate(last_first_row).RealValue
                    - 1387.0 / 360.0 * functions[i].Evaluate(last_second_row).RealValue
                    + 109.0 / 30.0 * functions[i].Evaluate(last_third_row).RealValue
                    - 637.0 / 360.0 * functions[i].Evaluate(last_four_row).RealValue
                    + 251.0 / 720.0 * functions[i].Evaluate(last_five_row).RealValue);
                new_ys.Add(current_y_name, new_y_value);
            }
            initialResult.Add(current_x, new_ys);
        }
        return initialResult;
    }
}
