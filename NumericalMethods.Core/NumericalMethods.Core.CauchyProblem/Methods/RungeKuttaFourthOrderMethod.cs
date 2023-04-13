using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
public class RungeKuttaFourthOrderMethod : IOneStepMethod
{
    public double Calculate(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
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
}
