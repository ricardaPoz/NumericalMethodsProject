using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
public class RungeKuttaThridOrderMethod : IOneStepMethod
{
    public double Calculate(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
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
}
