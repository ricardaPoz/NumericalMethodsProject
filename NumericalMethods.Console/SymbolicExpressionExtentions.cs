using MathNet.Symbolics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalMethods.Console
{
	public static class SymbolicExpressionExtentions
	{
		public static double EvaluateXY(this SymbolicExpression function, double valueX, double valueY)
		{
			return function.Evaluate(new Dictionary<string, FloatingPoint>() { { "x", valueX }, { "y", valueY } }).RealValue;
		}
	}
}
