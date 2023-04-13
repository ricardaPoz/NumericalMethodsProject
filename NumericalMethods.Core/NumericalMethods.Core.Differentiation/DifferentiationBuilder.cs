using NumericalMethods.Core.Differentiation.Interfaces;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions.Linear;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions.Quadratic;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions.NewtonPolynomial;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions.UndefinedCoefficients;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions.Cubic;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions.Runge;

namespace NumericalMethods.Core.Differentiation;
public static class DifferentiationBuilder
{
    public static IDifferentiationFunction? Build(IEnumerable<IDifferentiationNode> differentiation_nodes, DifferentiationFunctionType function_type, double step, int derrivative_degree)
    {
        return function_type switch
        {
            DifferentiationFunctionType.Linear => new LinearDifferentiationFunction(differentiation_nodes, step, derrivative_degree),
            DifferentiationFunctionType.Quadratic => new QuadraticDifferentationFunction(differentiation_nodes, step, derrivative_degree),
            DifferentiationFunctionType.Cubic => new CubicDifferentiationFunction(differentiation_nodes, step, derrivative_degree),
            _ => throw new NotImplementedException()
        };
    }

    public static INewtonDifferentiationFunction CreateNewton(IEnumerable<IDifferentiationNode> differentiationNodes, double step, int derivative_degree, int numberOfMembers)
    {
        return new NewtonPolynomialDifferentiationFunction(differentiationNodes, step, derivative_degree, numberOfMembers);
    }
    public static IUndefinedCoefficientsDifferentiationFunction CreateUndefinedCoefficients(IEnumerable<IDifferentiationNode> differentiationNodes, double step, int derrivative_degre, int count_coefficients_c)
    {
        return new UndefinedCoefficientsDifferentiationFunction(differentiationNodes, step, derrivative_degre, count_coefficients_c);
    }
    public static IDifferentiationFunction CreateRunge(IEnumerable<IDifferentiationNode> differentiationNodes, double step, int derrivative_degre, int accuracy_order, int number_of_used_points)
    {
        return new RungeDifferentiationFunction(differentiationNodes, step, derrivative_degre, accuracy_order, number_of_used_points);
    }
}