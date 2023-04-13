namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Shared;
public class VectorRow : VectorBase
{
	public VectorRow(int size) : base(size)
	{
	}

	public VectorRow(double[] data) : base(data)
	{
	}
	public VectorColumn Transposition()
    {
		return new VectorColumn(this._data);
    }
}
