namespace NumericalMethods.Core.Differentiation.DifferentiationFunctions.UndefinedCoefficients
{
	public class VectorColumn : VectorBase
	{
		public VectorColumn(int size) : base(size)
		{
		}

		public VectorColumn(double[] data) : base(data)
		{
		}

		public static VectorColumn operator -(VectorColumn vector)
		{
			for (int i = 0; i < vector.Size; i++)
			{
				vector[i] = -vector[i];
			}

			return vector;
		}

		public static VectorColumn operator +(VectorColumn vectorOne, VectorColumn vectorTwo)
		{
			// TODO: Попытаться переделать это
			if (vectorOne.Size > vectorTwo.Size)
			{
				VectorColumn result = new VectorColumn(vectorOne.Size);
				for (int i = 0; i < vectorTwo.Size; i++)
				{
					result[i] = vectorOne[i] + vectorTwo[i];
				}

				for (int i = vectorTwo.Size; i < result.Size; i++)
				{
					result[i] = vectorOne[i];
				}
				return result;
			} else
			{
				VectorColumn result = new VectorColumn(vectorTwo.Size);
				for (int i = 0; i < vectorOne.Size; i++)
				{
					result[i] = vectorOne[i] + vectorTwo[i];
				}

				for (int i = vectorOne.Size; i < result.Size; i++)
				{
					result[i] = vectorTwo[i];
				}
				return result;
			}
		}

		public static VectorColumn operator -(VectorColumn vectorOne, VectorColumn vectorTwo)
		{
			return vectorOne + -vectorTwo;
		}
	}
}
