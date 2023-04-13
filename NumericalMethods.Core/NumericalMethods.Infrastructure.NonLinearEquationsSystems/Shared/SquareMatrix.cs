using MathNet.Symbolics;

using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Exceptions;

namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Shared;
public class SquareMatrix
{
    private readonly double[,] _data;
    public int Size { get; set; }
    public SquareMatrix(int size)
    {
        InvariantException.ThrowIf(
            isViolated: size < 1,
            message: $"{nameof(size)} должен быть больше 1");

        _data = new double[size, size];
        Size = size;
    }
    public SquareMatrix(double[,] data)
    {
        InvariantException.ThrowIf(
            isViolated: data is null,
            message: $"{nameof(data)} не может быть null");
        InvariantException.ThrowIf(
            isViolated: data.GetLength(0) != data.GetLength(1),
            message: $"Размерности {nameof(data)} должны совпадать");

        Size = data.GetLength(0);
        _data = new double[Size, Size];
        FillDataFrom(data);
    }
    public double this[int row, int col]
    {
        get
        {
            return row >= 0 && row < _data.GetLength(0) && col >= 0 && col < _data.GetLength(1)
                ? _data[row, col]
                : throw new ArgumentOutOfRangeException("row or col invalid");
        }
        set
        {
            _data[row, col] = row >= 0 && row < _data.GetLength(0) && col >= 0 && col < _data.GetLength(1)
                ? value
                : throw new ArgumentOutOfRangeException("row or col invalid");
        }
    }
    public static SquareMatrix operator *(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        //Проверка чтобы у matrix1 колличество столбцов = колличество строк matrix2
        double[,] dataResultSquareMatrix = new double[matrix1.Size, matrix2.Size];//Здесь первый параметр должен быть кол-во строк первой матрицы, следующий кол-во столбцов матрицы 2
        double sum = 0;
        for (int i = 0; i < matrix1.Size; i++)
        {
            for (int j = 0; j < matrix1.Size; j++)
            {
                for (int k = 0; k < matrix2.Size; k++)
                {
                    sum += matrix1[i, k] * matrix2[k, j];
                }
                dataResultSquareMatrix[i, j] = sum;
                sum = 0;
            }

        }
        return new SquareMatrix(dataResultSquareMatrix);
    }
    public static SquareMatrix operator +(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        if (matrix1.Size != matrix2.Size) return null;
        double[,] dataResultSquareMatrix = new double[matrix1.Size, matrix2.Size];
        for (int i = 0; i < matrix1.Size; i++)
        {
            for (int j = 0; j < matrix1.Size; j++)
            {
                dataResultSquareMatrix[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }
        return new SquareMatrix(dataResultSquareMatrix);
    }
    public static SquareMatrix operator /(SquareMatrix matrix, double number)
    {
        double[,] resultDataSquareMatrix = new double[matrix.Size, matrix.Size];
        for (int i = 0; i < matrix.Size; i++)
        {
            for (int j = 0; j < matrix.Size; j++)
            {
                resultDataSquareMatrix[i, j] = matrix[i, j] / number;
            }
        }
        return new SquareMatrix(resultDataSquareMatrix);
    }
    public static VectorColumn operator *(SquareMatrix matrix, VectorColumn vector)
    {
        VectorColumn result = new VectorColumn(vector.Size);
        for (int i = 0; i < matrix.Size; i++)
        {
            double row_sum = 0;
            for (int j = 0; j < matrix.Size; j++)
            {
                row_sum += matrix[i, j] * vector[j];
            }
            result[i] = row_sum;
        }

        return result;
    }

    public static SquareMatrix operator -(SquareMatrix matrix)
    {
        for (int i = 0; i < matrix.Size; i++)
        {
            for (int j = 0; j < matrix.Size; j++)
            {
                matrix[i, j] = -matrix[i, j];
            }
        }

        return matrix;
    }

    private void FillDataFrom(double[,] data)
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                this[i, j] = data[i, j];
            }
        }
    }

    /// <summary> Вычисляет норму матрицы </summary>
    // TODO: Удалить при неиспользовании
    public double GetNorm()
    {
        double max = 0;

        for (int i = 0; i < Size; i++)
        {
            double row_sum = 0;
            for (int j = 0; j < Size; j++)
            {
                row_sum += Math.Abs(this[i, j]);
            }

            if (row_sum > max) max = row_sum;
        }

        return max;
    }

    /// <summary> Вычисляет определитель матрицы </summary>
    public double GetDeterminant()
    {
        int size = _data.GetLength(0);

        switch (size)
        {
            case 1: return _data[0, 0];
            case 2: return _data[0, 0] * _data[1, 1] - _data[1, 0] * _data[0, 1];
            default:
                double determinant = 0;
                for (int column = 0; column < size; column++)
                {
                    determinant += (column % 2 == 0 ? 1 : -1) * _data[0, column] * CreateMatrixWithoutRowAndColumn(0, column).GetDeterminant();
                }

                return determinant;
        }
    }

    /// <summary> Создает урезанную матрицу </summary>
    /// <param name="index_row">Удаляемая строка</param>
    /// <param name="index_column">Удаляемый столбец</param>
    /// <returns>Новая урезанная матрица</returns>
    public SquareMatrix CreateMatrixWithoutRowAndColumn(int index_row, int index_column)
    {
        SquareMatrix new_matrix = new SquareMatrix(Size - 1);

        for (int i = 0; i < new_matrix.Size; i++)
        {
            for (int j = 0; j < new_matrix.Size; j++)
            {
                if (i >= index_row && j >= index_column)
                    new_matrix[i, j] = this[i + 1, j + 1];
                else if (i >= index_row)
                    new_matrix[i, j] = this[i + 1, j];
                else if (j >= index_row)
                    new_matrix[i, j] = this[i, j + 1];
                else
                    new_matrix[i, j] = this[i, j];
            }
        }

        return new_matrix;
    }

    /// <summary> Инвертирует текущую матрицу</summary>
    public SquareMatrix Invert()
    {
        double determinant = GetDeterminant();
        SquareMatrix transposed = CreateAlgebraicAddition().Transpose();
        double[,] dataInvertSquareMatrix = new double[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                dataInvertSquareMatrix[i, j] = transposed[i, j] / determinant;
            }
        }
        return new SquareMatrix(dataInvertSquareMatrix);
    }

    /// <summary> Создает алгебраическое дополнение </summary>
    /// <returns>Новая матрица - алгебраическое дополнение текущей</returns>
    public SquareMatrix CreateAlgebraicAddition()
    {
        SquareMatrix algebraic_addition_matrix = new SquareMatrix(Size);
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                algebraic_addition_matrix[i, j] = ((i + j) % 2 is 0 ? 1 : -1) * CreateMatrixWithoutRowAndColumn(i, j).GetDeterminant();
            }
        }
        return algebraic_addition_matrix;
    }

    /// <summary> Транспонирует текущую матрицу </summary>
    public SquareMatrix Transpose()
    {
        double[,] new_data = new double[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                new_data[i, j] = this[j, i];
            }
        }

        FillDataFrom(new_data);
        return this;
    }

    /// <summary> Создает матрицу Якоби </summary>
    public static SquareMatrix CreateJacobiMatrix(IEnumerable<SymbolicExpression> functions, Dictionary<string, FloatingPoint> values)
    {
        var variables = functions.First().CollectVariables();
        int count_params = variables.Count();
        SquareMatrix resultMatrix = new SquareMatrix(count_params);
        for (int i = 0; i < resultMatrix.Size; i++)
        {
            for (int j = 0; j < resultMatrix.Size; j++)
            {
                string currentVariableName = variables.ElementAt(j).ToString();
                resultMatrix[i, j] = functions.ElementAt(i)
                    .Differentiate(variables.ElementAt(j))
                    .Evaluate(values).RealValue;
            }
        }

        return resultMatrix;
    }
}
