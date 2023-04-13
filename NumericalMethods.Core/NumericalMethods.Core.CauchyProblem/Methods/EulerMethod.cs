using MathNet.Symbolics;
using NumericalMethods.Core.CauchyProblem.Interfaces;

namespace NumericalMethods.Core.CauchyProblem.Methods;
internal class EulerMethod : IOneStepMethod
{
    public double Calculate(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions)
    {
        return yi.value + h * function.Evaluate(conditions).RealValue;
    }
}
