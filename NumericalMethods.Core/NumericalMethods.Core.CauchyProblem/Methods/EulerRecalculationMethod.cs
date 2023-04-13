using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
internal class EulerRecalculationMethod : IOneStepMethod
{
    public double Calculate(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
    {
        Dictionary<string, FloatingPoint> conditionsCopy = conditions.ToDictionary(pair => pair.Key, pair => pair.Value);
        double functionValue = function.Evaluate(conditionsCopy).RealValue;
        double y_next_approximation = yi.value + h * functionValue;
        conditionsCopy["x"] = x + h;
        conditionsCopy[yi.name] = y_next_approximation;
        return yi.value + (h * (functionValue + function.Evaluate(conditionsCopy).RealValue)) / 2;
    }
}
