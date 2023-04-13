using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
internal class EulerIterativeMethod : IOneStepMethod
{
    public double Calculate(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
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
}
