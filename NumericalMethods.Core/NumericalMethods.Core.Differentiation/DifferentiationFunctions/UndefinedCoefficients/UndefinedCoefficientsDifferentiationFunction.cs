using MathNet.Symbolics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumericalMethods.Core.Differentiation.Interfaces;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions;

namespace NumericalMethods.Core.Differentiation.DifferentiationFunctions.UndefinedCoefficients
{
    internal class UndefinedCoefficientsDifferentiationFunction : DifferentiationFunctionBase, IUndefinedCoefficientsDifferentiationFunction
    {
        private readonly int _count_coefficients_c;
        SymbolicExpression[] _ys_before_last_derivative_expressions;
        SymbolicExpression[] _ys_last_derivative_expressions;
        private double[] _current_xs;
        private double[] _current_ys;
        public UndefinedCoefficientsDifferentiationFunction(IEnumerable<IDifferentiationNode> differentiationNodes, double step, int derrivative_degree, int count_coefficients_c)
            : base(differentiationNodes, step, derrivative_degree)
        {
            _count_coefficients_c = count_coefficients_c;
            _current_xs = differentiationNodes.Select(node => node.X).ToArray();
            _current_ys = differentiationNodes.Select(node => node.Y).ToArray();
            _ys_before_last_derivative_expressions = new SymbolicExpression[_count_coefficients_c];
            _ys_last_derivative_expressions = new SymbolicExpression[_count_coefficients_c];
            for (int i = 0; i < _count_coefficients_c; i++)
            {
                _ys_before_last_derivative_expressions[i] = $"(x - x0)^{i}";
            }
            _ys_last_derivative_expressions = _ys_before_last_derivative_expressions.Select(y => y.Differentiate("x")).ToArray();
        }
        public IEnumerable<IDifferentiationResultNode> Calculate()
        {
            for (int i = 0; i < _derivative_degree; i++)
            {
                List<double> cs = SolveLinearSystem().ToList();
                int start_new_items_index = 1;
                int end_new_items_index = _current_xs.Length - (_count_coefficients_c - 2);
                double[] new_ys = new double[end_new_items_index];
                double[] new_xs = _current_xs[start_new_items_index..end_new_items_index];
                for (int j = start_new_items_index; j < end_new_items_index; j++)
                {
                    new_ys[j - 1] = _current_ys[(j - 1)..(j - 1 + _count_coefficients_c)].Zip(cs, (y, c) => y * c).Sum();
                }
                _current_xs = new_xs;
                _current_ys = new_ys;
            }
            return _current_xs.Zip(_current_ys, (x, y) => new DifferentiationResultNode(x, y));
        }
        private VectorColumn SolveLinearSystem()
        {
            SquareMatrix matrix = CreateMatrix();
            VectorColumn vectorB = CreateVectorB();

            SquareMatrix invertedMatrix = matrix.Invert();
            return invertedMatrix * vectorB;
        }
        private VectorColumn CreateVectorB()
        {
            IDictionary<string, FloatingPoint> values = new Dictionary<string, FloatingPoint>()
            {
                { "x", _current_xs[1] },
                { "x0", _current_xs[0] }
            };

            double[] vectorBody = _ys_last_derivative_expressions
                .Select((yExpression, index) => yExpression.Evaluate(values).RealValue)
                .ToArray();

            return new VectorColumn(vectorBody);
        }
        private SquareMatrix CreateMatrix()
        {
            double[,] matrixBody = new double[_count_coefficients_c, _count_coefficients_c];
            for (int i = 0; i < _count_coefficients_c; i++)
            {
                for (int j = 0; j < _count_coefficients_c; j++)
                {
                    IDictionary<string, FloatingPoint> values = new Dictionary<string, FloatingPoint>()
                    {
                        { "x", _current_xs[j] },
                        { "x0", _current_xs[0] }
                    };
                    matrixBody[i, j] = _ys_before_last_derivative_expressions[i].Evaluate(values).RealValue;
                }
            }

            return new SquareMatrix(matrixBody);
        }
    }
}
