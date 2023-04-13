using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Symbolics;

namespace NumericalMethods.Console
{
    public static class TestDifferentialEquations
    {
        public static void Run()
        {
            ResultTable resultTwo = Calculate(
            function: "2*x",
            b: 10,
            h: 1,
            initialGuess: (x: 1, ys: new Dictionary<string, double>()
            {
                            {"y0", 1},
            }));
            ResultTable result = Calculate(
            function: "x - 2*y1 - y0",
            b: 1,
            h: 0.2,
            initialGuess: (x: 0, ys: new Dictionary<string, double>()
            {
                {"y0", 1},
                {"y1", 0}
            }));
            ResultTable resultThree = Calculate(
            function: "2*(x^2 + y0)",
            b: 1,
            h: 0.1,
            initialGuess: (x: 0, ys: new Dictionary<string, double>()
            {
                {"y0", 1},
            }));
            ResultTable resultFour = Calculate(
            function: "3*x^2*y0 + x^2*e^(x^3)",
            b: 1,
            h: 0.1,
            initialGuess: (x: 0, ys: new Dictionary<string, double>()
            {
                 {"y0", 0},
            }));
            ResultTable resultFive = Calculate(
            function: "x - y0",
            b: 0.1,
            h: 0.01,
            initialGuess: (x: 0, ys: new Dictionary<string, double>()
            {
                  {"y0", 1},
                  {"y1", 0}
            }));
            ResultTable resultSix = Calculate(
            function: "x - y0 - 0*y1",
            b: 0.1,
            h: 0.01,
            initialGuess: (x: 0, ys: new Dictionary<string, double>()
            {
                              {"y0", 1},
                              {"y1", 1},
                              {"y2", 1},
                              {"y3", 1},
                              {"y4", 1},
                              {"y5", 1}
            }));
        }
        private static double EulerMethod(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
        {
            return yi.value + h * function.Evaluate(conditions).RealValue;
        }
        private static double EulerMethodRecalculation(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
        {
            Dictionary<string, FloatingPoint> conditionsCopy = conditions.ToDictionary(pair => pair.Key, pair => pair.Value);
            double functionValue = function.Evaluate(conditionsCopy).RealValue;
            double y_next_approximation = yi.value + h * functionValue;
            conditionsCopy["x"] = x + h;
            conditionsCopy[yi.name] = y_next_approximation;
            return yi.value + (h * (functionValue + function.Evaluate(conditionsCopy).RealValue)) / 2;
        }
        private static double EulerIterativeMethod(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
        {
            Dictionary<string, FloatingPoint> conditionsCopy = conditions.ToDictionary(pair => pair.Key, pair => pair.Value);
            double functionValue = function.Evaluate(conditionsCopy).RealValue;
            double y_next = yi.value + h * functionValue;
            conditionsCopy["x"] = x + h;
            for (int k = 0; k < 4; k++)
            {
                conditionsCopy[yi.name] = y_next;
                y_next = yi.value + h * (functionValue + function.Evaluate(conditionsCopy).RealValue) / 2;
            }
            return y_next;
        }
        private static double EulerMethodImproved(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
        {
            Dictionary<string, FloatingPoint> conditionsCopy = conditions.ToDictionary(pair => pair.Key, pair => pair.Value);
            double y_half = yi.value + h / 2 * function.Evaluate(conditionsCopy).RealValue;
            conditionsCopy["x"] = x + h / 2;
            conditionsCopy[yi.name] = y_half;
            return yi.value + h * function.Evaluate(conditionsCopy).RealValue;
        }
        private static double RungeKuttaThridOrderMethods(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
        {
            Dictionary<string, FloatingPoint> conditionsCopy = conditions.ToDictionary(pair => pair.Key, pair => pair.Value);

            double y_delta, k_one, k_two, k_three;

            k_one = h * function.Evaluate(conditionsCopy).RealValue;
            conditionsCopy["x"] = x + h / 3;
            conditionsCopy[yi.name] = yi.value + k_one / 3;
            k_two = h * function.Evaluate(conditionsCopy).RealValue;
            conditionsCopy["x"] = x + h * 2.0 / 3.0;
            conditionsCopy[yi.name] = yi.value + k_two * 2.0 / 3.0;
            k_three = h * function.Evaluate(conditionsCopy).RealValue;
            y_delta = (k_one + 3 * k_three) / 4;
            return yi.value + y_delta;
        }
        private static double RungeKuttaFourthOrderMethods(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
        {
            Dictionary<string, FloatingPoint> conditionsCopy = conditions.ToDictionary(pair => pair.Key, pair => pair.Value);
            double y_delta, k_one, k_two, k_three, k_four;
            k_one = h * function.Evaluate(conditionsCopy).RealValue;
            conditionsCopy["x"] = x + h / 2;
            conditionsCopy[yi.name] = yi.value + k_one / 2;
            k_two = h * function.Evaluate(conditionsCopy).RealValue;
            conditionsCopy[yi.name] = yi.value + k_two / 2;
            k_three = h * function.Evaluate(conditionsCopy).RealValue;
            conditionsCopy["x"] = x + h;
            conditionsCopy[yi.name] = yi.value + k_three;
            k_four = h * function.Evaluate(conditionsCopy).RealValue;
            y_delta = (k_one + 2 * k_two + 2 * k_three + k_four) / 6;
            return yi.value + y_delta;
        }
        private static ResultTable AdamsMethod(List<SymbolicExpression> functions, double b, double h, ResultTable initialResult)
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
        private static ResultTable AdamsBashforthMethod(List<SymbolicExpression> functions, double b, double h, ResultTable initialResult)
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
                        - 1.0/2.0 * functions[i].Evaluate(first_row).RealValue);
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
                    double new_y_value = current_y_value + h * (1901.0/720.0*functions[i].Evaluate(last_first_row).RealValue
                        - 1387.0/360.0* functions[i].Evaluate(last_second_row).RealValue
                        + 109.0/30.0* functions[i].Evaluate(last_third_row).RealValue
                        - 637.0/360.0* functions[i].Evaluate(last_four_row).RealValue
                        + 251.0/720.0* functions[i].Evaluate(last_five_row).RealValue);
                    new_ys.Add(current_y_name, new_y_value);
                }
                initialResult.Add(current_x, new_ys);
            }
            return initialResult;
        }
        private static ResultTable AdamsMoultonMethod(List<SymbolicExpression> functions, double b, double h, ResultTable initialResult)
        {
            Func<Dictionary<string, double>, Dictionary<string, FloatingPoint>> convert_with_floating_point =
                (row) => row
                    .Select(pair => new KeyValuePair<string, FloatingPoint>(pair.Key, pair.Value))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

            ResultTable filledResultTable = AdamsBashforthMethod(functions, b, h, initialResult);
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
        private static ResultTable Calculate(string function, double b, double h, (double x, Dictionary<string, double> ys) initialGuess)
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
            for (double current_x = initialGuess.x + h; Math.Round(current_x, 7) <= Math.Round(initialGuess.x + 3 * h, 7); current_x += h)
            {
                Dictionary<string, double> ys = new Dictionary<string, double>();
                Dictionary<string, FloatingPoint> conditions = new Dictionary<string, FloatingPoint>() { { "x", current_x - h } };
                foreach (var yName in initialGuess.ys.Keys)
                {
                    conditions.Add(yName, result[yName].Last().yi);
                }
                foreach (KeyValuePair<string, double> y in initialGuess.ys)
                {
                    var newValueY = RungeKuttaFourthOrderMethods(SymbolicExpression.Parse(functions[y.Key]), h, current_x - h, (y.Key, conditions[y.Key].RealValue), conditions);
                    ys.Add(y.Key, newValueY);
                }
                result.Add(current_x, ys);
            }
            return AdamsBashforthMethod(
                functions: functions.Select(pair => SymbolicExpression.Parse(pair.Value)).ToList(),
                b: b,
                h: h,
                result);
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
                    var newValueY = EulerIterativeMethod(SymbolicExpression.Parse(functions[y.Key]), h, current_x - h, (y.Key, conditions[y.Key].RealValue), conditions);
                    ys.Add(y.Key, newValueY);
                }
                result.Add(current_x, ys);
            }
            return result;
        }
    }
}
