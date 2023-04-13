using MathNet.Symbolics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalMethods.Core.CauchyProblem.Interfaces
{
    public interface IMultiStepMethod
    {
        ResultTable Calculate(List<SymbolicExpression> functions, double b, double h, ResultTable initialResult);
    }
}
