using MathNet.Symbolics;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions;
using NumericalMethods.Core.Differentiation.Interfaces;

using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NumericalMethods.Core.Differentiation.DifferentiationFunctions.NewtonPolynomial;
internal class NewtonPolynomialDifferentiationFunction : DifferentiationFunctionBase, INewtonDifferentiationFunction
{
    private readonly Dictionary<int, int> factorialCache = new Dictionary<int, int>() { { 1, 1 } };
    private int _numberOfMembers;
    private SymbolicExpression _functionExpression;
    private List<string> _other_variables;
    private List<int> _degrees;
    private readonly double last_x_for_right_finite_difference;
    private readonly double first_x_for_left_finite_difference;
    public NewtonPolynomialDifferentiationFunction(IEnumerable<IDifferentiationNode> differentiationNodes, double step, int derrivative_degree, int numberOfMembers)
        : base(differentiationNodes, step, derrivative_degree)
    {
        _numberOfMembers = numberOfMembers;
        string? function = GetNewtonPolynomialDerrivative(_derivative_degree, _numberOfMembers);
        _functionExpression = SymbolicExpression.Parse(function);
        _other_variables = _functionExpression
            .CollectVariables()
            .Select(variable => variable.ToString())
            .Where(variable => variable.Contains('y')).ToList();
        _degrees = _other_variables
            .Select(variable => int.Parse(variable[1..])).ToList();

        int max_finite_difference_degree = _numberOfMembers - 1;
        last_x_for_right_finite_difference = _first_node.X + step * max_finite_difference_degree;
        first_x_for_left_finite_difference = _last_node.X - step * max_finite_difference_degree;
    }
    private int CalculateFactorial(int value)
    {
        if (factorialCache.TryGetValue(value, out int cachedValue))
        {
            return cachedValue;
        }
        else
        {
            if (value < 2) return 1;
            int calculatedValue = value * CalculateFactorial(value - 1);
            factorialCache.Add(value, calculatedValue);
            return calculatedValue;
        }
    }
    private string GetNewtonPolynomial(int numberOfMembers)
    {
        switch (numberOfMembers)
        {
            case < 2: return "f";
            case 2: return "f + t*y1";
            default:
                {
                    StringBuilder result = new StringBuilder("f + t*y1");
                    for (int i = 3; i <= numberOfMembers; i++)
                    {
                        int n = i - 1;
                        int factorial = CalculateFactorial(n);
                        List<string> bracketsList = new List<string>();
                        for (int j = 0; j < n - 1; j++)
                        {
                            bracketsList.Add($"(t - {j + 1})");
                        }

                        string bracketsString = string.Join(" * ", bracketsList);
                        string member = $"t * {bracketsString}/{factorial}*y{n}";
                        result.Append($" + {member}");
                    }

                    return result.ToString();
                }
        }
    }
    private string? GetNewtonPolynomialDerrivative(int derrivativeDegree, int numberOfMembers)
    {
        if (derrivativeDegree < 1 || derrivativeDegree + 1 > numberOfMembers) return null;
        string newtonPolynomial = GetNewtonPolynomial(numberOfMembers);
        SymbolicExpression result = SymbolicExpression.Parse(newtonPolynomial).Expand();
        for (int i = 0; i < derrivativeDegree; i++)
        {
            result = result.Differentiate("t");
        }

        return result.ToString();
    }
    public double? Calculate(double argument)
    {
        List<double?> variablesValues = new List<double?>();
        FiniteDifferences difference_type = ChoiceDifference(argument);
        double t_value = 1;
        double denominator = 1;

        switch (difference_type)
        {
            case FiniteDifferences.Right:
                variablesValues = _degrees.Select(finiteDifferenceDegree => GetRightFiniteDifference(argument, finiteDifferenceDegree)).ToList();
                t_value = 0;
                denominator = Math.Pow(_step, _derivative_degree);
                break;
            case FiniteDifferences.Left:
                variablesValues = _degrees.Select(finiteDifferenceDegree => GetLeftFiniteDifference(argument, finiteDifferenceDegree)).ToList();
                t_value = 1;
                denominator = Math.Pow(_step, _derivative_degree);
                break;
            case FiniteDifferences.Center:
                variablesValues = _degrees.Select(finiteDifferenceDegree => GetCenterFiniteDifference(argument, finiteDifferenceDegree)).ToList();
                t_value = 1;
                denominator = Math.Pow(2 * _step, _derivative_degree);
                break;
        };
        if (variablesValues.Any(variableValue => variableValue is null)) return null;

        Dictionary<string, FloatingPoint> values = new Dictionary<string, FloatingPoint>()
            {
                {"t", t_value }
            };
        for (int i = 0; i < _other_variables.Count; i++)
        {
            values.Add(_other_variables[i], variablesValues[i]);
        }

        return _functionExpression.Evaluate(values).RealValue / denominator;
    }
    private FiniteDifferences ChoiceDifference(double argument)
    {
        if (argument <= last_x_for_right_finite_difference) return FiniteDifferences.Right;
        else if (argument >= first_x_for_left_finite_difference) return FiniteDifferences.Left;
        else return FiniteDifferences.Center;
    }
}
