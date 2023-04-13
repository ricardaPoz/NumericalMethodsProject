namespace NumericalMethods.Core.Approximation.InterpolationFunctions.Linear;
internal record class LinearFunction(double A, double B, double StartRangeX)
{
	public double Calculate(double argument) => A + (B * (argument - StartRangeX));
}
