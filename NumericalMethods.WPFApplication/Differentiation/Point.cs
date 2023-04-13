using NumericalMethods.Core.Approximation.Interfaces;
using NumericalMethods.Core.Differentiation.Interfaces;

namespace NumericalMethods.WPFApplication.Differentiation;
public record class Point(double X, double Y) : IInterpolationNode, IDifferentiationNode;