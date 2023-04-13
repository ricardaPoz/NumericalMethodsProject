using MathNet.Symbolics;
using NumericalMethods.Infrastructure.Integration.Shared;

namespace NumericalMethods.Infrastructure.Integration.Methods.MonteCarlo
{
    internal class MonteCarloIntegrationMethod : IIntegrationMonteCarloMethod
    {
        public double Integrate(string function, double start, double end, int count_points)
        {
            SymbolicExpression func = SymbolicExpression.Parse(function);
            Random rnd = new Random();
            double x = start, sumValuesFunctions = 0;
            for (int i = 0; i < count_points; i++)
            {
                while(x <= start || x > end)
                    x = rnd.Next((int)start, (int)end) + rnd.NextDouble();
                sumValuesFunctions += func.EvaluateX(x);
                x = start;
            }
            return (end - start) / count_points * sumValuesFunctions;
        }
    }
}
