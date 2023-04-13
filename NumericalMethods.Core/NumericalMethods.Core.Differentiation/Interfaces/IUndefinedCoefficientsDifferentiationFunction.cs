using NumericalMethods.Core.Differentiation.DifferentiationFunctions.UndefinedCoefficients;
namespace NumericalMethods.Core.Differentiation.Interfaces;
public interface IUndefinedCoefficientsDifferentiationFunction
{
    public IEnumerable<IDifferentiationResultNode> Calculate();
}
