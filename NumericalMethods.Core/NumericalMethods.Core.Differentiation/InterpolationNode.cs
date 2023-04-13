using NumericalMethods.Core.Approximation.Interfaces;

namespace NumericalMethods.Core.Differentiation;
public record class InterpolationNode(double X, double Y) : IInterpolationNode;
