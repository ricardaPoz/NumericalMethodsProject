using MathNet.Symbolics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalMethods.Core.CauchyProblem.Interfaces
{
    public interface IOneStepMethod
    {
        double Calculate(SymbolicExpression function, double h, double x, (string name, double value) yi, Dictionary<string, FloatingPoint> conditions);
    }
}
