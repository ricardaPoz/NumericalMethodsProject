using NumericalMethods.Core.Differentiation.Interfaces;

namespace NumericalMethods.Core.Differentiation.DifferentiationFunctions.Cubic;
internal class CubicDifferentiationFunction : DifferentiationFunctionBase, IDifferentiationFunction
{
    private readonly INewtonDifferentiationFunction _newton_function;
    public CubicDifferentiationFunction(IEnumerable<IDifferentiationNode> differentiationNodes, double step, int derrivative_degree) 
        : base(differentiationNodes, step, derrivative_degree)
    {
        _newton_function = DifferentiationBuilder.CreateNewton(differentiationNodes, step, derrivative_degree, derrivative_degree + 3);
    }

    public double? Calculate(double argument) => _newton_function.Calculate(argument);
}
