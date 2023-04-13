using MathNet.Symbolics;
using MathNet.Numerics;

namespace NumericalMethods.Infrastructure.Integration.Shared
{
    internal static class SymbolicExpressionExtentions
    {
        public static double EvaluateX(this SymbolicExpression function,double valueX)
        {
            return function.Evaluate(new Dictionary<string, FloatingPoint>() { { "x", valueX } }).RealValue;
        }
        public static double Derivative(this SymbolicExpression function,double valueX, int order)
        {
            return Differentiate.Derivative(x => function.EvaluateX(x), valueX, order);
        }
        
    }
}
