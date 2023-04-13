using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
public class AdamsMoultonMethod : IMultiStepMethod
{
    public ResultTable Calculate(List<SymbolicExpression> functions, double b, double h, ResultTable initialResult)
    {
        Func<Dictionary<string, double>, Dictionary<string, FloatingPoint>> convert_with_floating_point =
            (row) => row
                .Select(pair => new KeyValuePair<string, FloatingPoint>(pair.Key, pair.Value))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        ResultTable filledResultTable = new AdamsBashforthMethod().Calculate(functions, b, h, initialResult);
        Dictionary<string, FloatingPoint> second_row = convert_with_floating_point(filledResultTable.GetRow(1));
        for (int i = 0; i < functions.Count; i++)
        {
            KeyValuePair<string, double> previous_y = filledResultTable.GetRow(0).ElementAt(i + 1);
            double new_y_value = previous_y.Value + h * functions[i].Evaluate(second_row).RealValue;
            filledResultTable.EditRowItem(1, previous_y.Key, new_y_value);
        }
        Dictionary<string, FloatingPoint> third_row = convert_with_floating_point(filledResultTable.GetRow(2));
        for (int i = 0; i < functions.Count; i++)
        {
            KeyValuePair<string, FloatingPoint> previous_y = second_row.ElementAt(i + 1);
            double new_y_value = previous_y.Value.RealValue + 1.0 / 2.0 * h
                * (functions[i].Evaluate(third_row).RealValue + functions[i].Evaluate(second_row).RealValue);
            filledResultTable.EditRowItem(2, previous_y.Key, new_y_value);
        }
        Dictionary<string, FloatingPoint> four_row = convert_with_floating_point(filledResultTable.GetRow(3));
        for (int i = 0; i < functions.Count; i++)
        {
            KeyValuePair<string, FloatingPoint> previous_y = third_row.ElementAt(i + 1);
            double new_y_value = previous_y.Value.RealValue + h
                * (5.0 / 12.0 * functions[i].Evaluate(four_row).RealValue
                + 2.0 / 3.0 * functions[i].Evaluate(third_row).RealValue
                - 1.0 / 12.0 * functions[i].Evaluate(second_row).RealValue);
            filledResultTable.EditRowItem(3, previous_y.Key, new_y_value);
        }
        Dictionary<string, FloatingPoint> five_row = convert_with_floating_point(filledResultTable.GetRow(4));
        for (int i = 0; i < functions.Count; i++)
        {
            KeyValuePair<string, FloatingPoint> previous_y = four_row.ElementAt(i + 1);
            double new_y_value = previous_y.Value.RealValue + h
                * (3.0 / 8.0 * functions[i].Evaluate(five_row).RealValue
                + 19.0 / 24.0 * functions[i].Evaluate(four_row).RealValue
                - 5.0 / 24.0 * functions[i].Evaluate(third_row).RealValue
                + 1.0 / 24.0 * functions[i].Evaluate(second_row).RealValue);
            filledResultTable.EditRowItem(4, previous_y.Key, new_y_value);
        }


        for (int row_index = 5; row_index < filledResultTable.CountRow; row_index++)
        {
            Dictionary<string, FloatingPoint> previous_four_row = convert_with_floating_point(filledResultTable.GetRow(row_index - 4));
            Dictionary<string, FloatingPoint> previous_third_row = convert_with_floating_point(filledResultTable.GetRow(row_index - 3));
            Dictionary<string, FloatingPoint> previous_two_row = convert_with_floating_point(filledResultTable.GetRow(row_index - 2));
            Dictionary<string, FloatingPoint> previous_one_row = convert_with_floating_point(filledResultTable.GetRow(row_index - 1));
            Dictionary<string, FloatingPoint> current_row = convert_with_floating_point(filledResultTable.GetRow(row_index));
            for (int i = 0; i < functions.Count; i++)
            {
                KeyValuePair<string, FloatingPoint> previous_y = previous_one_row.ElementAt(i + 1);
                double new_y_value = previous_y.Value.RealValue + h
                    * (251.0 / 720.0 * functions[i].Evaluate(current_row).RealValue
                    + 646.0 / 720.0 * functions[i].Evaluate(previous_one_row).RealValue
                    - 264.0 / 720.0 * functions[i].Evaluate(previous_two_row).RealValue
                    + 106.0 / 720.0 * functions[i].Evaluate(previous_third_row).RealValue
                    - 19.0 / 720.0 * functions[i].Evaluate(previous_four_row).RealValue);
                filledResultTable.EditRowItem(row_index, previous_y.Key, new_y_value);
            }
        }
        return filledResultTable;
    }
}
