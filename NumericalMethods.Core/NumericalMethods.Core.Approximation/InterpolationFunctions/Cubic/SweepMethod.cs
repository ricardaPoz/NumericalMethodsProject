namespace NumericalMethods.Core.Approximation.InterpolationFunctions.Cubic;
internal record class SweepMethod(double[] Left, double[] Center, double[] Right, double[] FreeMembers)
{
	public List<double> Calculate()
	{
		List<(double a, double b)> coefficients = new List<(double a, double b)>() { (-Right[0] / Center[0], FreeMembers[0] / Center[0]) };
		for (int i = 1; i < FreeMembers.Length - 1; i++)
		{
			double e = (Left[i - 1] * coefficients[i - 1].a) + Center[i];
			double a = -Right[i] / e;
			double b = (FreeMembers[i] - (Left[i - 1] * coefficients[i - 1].b)) / e;
			coefficients.Add((a, b));
		}

		List<double> result = new List<double>()
			{
				(FreeMembers[^1] - (Left[^1] * coefficients[^1].b)) /
				(Center[^1] + (Left[^1] * coefficients[^1].a))
			};
		coefficients.Reverse();
		for (int i = 0; i < coefficients.Count; i++)
		{
			double x = (coefficients[i].a * result[i]) + coefficients[i].b;
			result.Add(x);
		}

		result.Reverse();
		return result;
	}
}
