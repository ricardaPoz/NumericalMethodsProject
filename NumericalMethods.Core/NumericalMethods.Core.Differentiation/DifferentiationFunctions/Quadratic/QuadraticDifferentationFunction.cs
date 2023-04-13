using NumericalMethods.Core.Differentiation;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions;
using NumericalMethods.Core.Differentiation.Interfaces;

namespace NumericalMethods.Core.Differentiation.DifferentiationFunctions.Quadratic;
internal class QuadraticDifferentationFunction : DifferentiationFunctionBase, IDifferentiationFunction
{
    private INewtonDifferentiationFunction _newton_function;
    public QuadraticDifferentationFunction(IEnumerable<IDifferentiationNode> differentiation_nodes, double step, int derrivative_degree)
        : base(differentiation_nodes, step, derrivative_degree)
    {
        _newton_function = DifferentiationBuilder.CreateNewton(differentiation_nodes, step, derrivative_degree, derrivative_degree + 2);
    }

    public double? Calculate(double argument) => _newton_function.Calculate(argument);
}
