using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
public class EulerImprovedMethod : IOneStepMethod
{
    public double Calculate(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
    {
        Dictionary<string, FloatingPoint> conditionsCopy = conditions.ToDictionary(pair => pair.Key, pair => pair.Value);
        double y_half = yi.value + h / 2 * function.Evaluate(conditionsCopy).RealValue;
        conditionsCopy["x"] = x + h / 2;
        conditionsCopy[yi.name] = y_half;
        return yi.value + h * function.Evaluate(conditionsCopy).RealValue;
    }
}