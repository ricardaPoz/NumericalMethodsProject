namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Shared
{
    public class VectorColumn : VectorBase
	{
		public VectorColumn(int size) : base(size)
		{
		}

		public VectorColumn(double[] data) : base(data)
		{
		}
		public VectorRow Transposition()
        {
			return new VectorRow(this._data);
        }
		public static VectorColumn operator -(VectorColumn vector)
		{
			for (int i = 0; i < vector.Size; i++)
			{
				vector[i] = -vector[i];
			}

			return vector;
		}
		public static SquareMatrix operator *(VectorColumn vectorColumn, VectorRow vectorRow)
        {
			if (vectorColumn.Size != vectorRow.Size) return null;
			double[,] dataSquareMatrix = new double[vectorRow.Size, vectorColumn.Size];
            for (int i = 0; i < vectorRow.Size; i++)
            {
                for (int j = 0; j < vectorColumn.Size; j++)
                {
					dataSquareMatrix[i,j] = vectorColumn[i] * vectorRow[j];
                }
            }
			return new SquareMatrix(dataSquareMatrix);
        }
		public static double? operator *(VectorRow vectorRow, VectorColumn vectorColumn)
		{
			if (vectorColumn.Size != vectorRow.Size) return null;
			double result = 0;
			for (int i = 0; i < vectorRow.Size; i++)
			{
				result += vectorRow[i] * vectorColumn[i];
			}
			return result;
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
