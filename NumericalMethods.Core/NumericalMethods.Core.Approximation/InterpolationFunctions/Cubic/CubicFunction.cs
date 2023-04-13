namespace NumericalMethods.Core.Approximation.InterpolationFunctions.Cubic;
internal record class CubicFunction(double A, double B, double C, double D, double StartX)
{
	public double Calculate(double argument)
		=> A + (B * (argument - StartX)) + (C * Math.Pow(argument - StartX, 2)) + (D * Math.Pow(argument - StartX, 3));
}
