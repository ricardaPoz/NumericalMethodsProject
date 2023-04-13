using NumericalMethods.Core.Differentiation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalMethods.Core.Differentiation.DifferentiationFunctions.Runge
{
    internal class RungeDifferentiationFunction : DifferentiationFunctionBase, IDifferentiationFunction
    {
        private readonly int _accuracy_order;
        private readonly int _number_of_used_points;
        public RungeDifferentiationFunction(IEnumerable<IDifferentiationNode> differentiationNodes, double step, int derrivative_degree, int accuracy_order, int number_of_used_points) 
            : base(differentiationNodes, step, derrivative_degree)
        {
            _accuracy_order = accuracy_order;
            _number_of_used_points = number_of_used_points;
        }

        public double? Calculate(double argument)
        {
            return CalculateRecursive(argument, 1);
        }
        private double? CalculateRecursive(double argument,int depth)
        {
            Func<double, double?> f = _interpolation_function.Calculate;
            double? xh = (f(argument) - f(argument - _step)) / _step;
            double? xkh = (f(argument) - f(argument - _step * _number_of_used_points)) / (_step * _number_of_used_points);
            if (xh == null || xkh == null)
            {
                xh = (f(argument + _step) - f(argument)) / _step;
                xkh = (f(argument + _step * _number_of_used_points) - f(argument)) / (_step * _number_of_used_points);
            }
            int new_depth = depth + 1;
            if(depth != _derivative_degree)
            {
                xh = (CalculateRecursive(argument, new_depth) - CalculateRecursive(argument - _step, new_depth)) / _step;
                xkh = (CalculateRecursive(argument, new_depth) - CalculateRecursive(argument - _step * _number_of_used_points, new_depth)) / (_step * _number_of_used_points);
                if (xh == null || xkh == null)
                {
                    xh = (CalculateRecursive(argument+_step,new_depth) - CalculateRecursive(argument,new_depth)) / _step;
                    xkh = (CalculateRecursive(argument + _step * _number_of_used_points,new_depth) - CalculateRecursive(argument, new_depth)) / (_step * _number_of_used_points);
                }
            }
            return xh + (xh - xkh) / (Math.Pow(_number_of_used_points, _accuracy_order) - 1);

        }
    }
}
