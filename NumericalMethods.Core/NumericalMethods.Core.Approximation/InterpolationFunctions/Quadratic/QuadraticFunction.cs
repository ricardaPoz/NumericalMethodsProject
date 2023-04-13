namespace NumericalMethods.Core.Approximation.InterpolationFunctions.Quadratic;
internal record class QuadraticFunction(double A, double B, double C)
{
	public double Calculate(double argument) => A * Math.Pow(argument, 2) + B * argument + C;
}