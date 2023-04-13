using MathNet.Symbolics;
using NumericalMethods.Infrastructure.Integration.Shared;

namespace NumericalMethods.Infrastructure.Integration.Methods.Gauss
{
    internal class GaussIntegrationMethod : IIntegrationMethodWithVariableStep
    {
        public double Integrate(string function, double start, double end, int count_nodes)
        {
            SymbolicExpression func = SymbolicExpression.Parse(function);
            double[] acoef = new double[count_nodes];
            double[] x = new double[count_nodes];
            for (int i = 1; i <= count_nodes; i++)
            {
                double ti = T(count_nodes - 1, i, count_nodes);
                double pd = PD(count_nodes, ti);
                double diva = (1 - ti * ti) * pd * pd;
                acoef[i - 1] = 2 / diva;
                x[i - 1] = GetRealX(start, end, ti);
            }
            return (end - start) / 2 * acoef.Zip(x, (ac, xc) => ac * func.EvaluateX(xc)).Sum();
        }
        double GetRealX(double a, double b, double t)
        {
            return (a + b) / 2 + (b - a) / 2 * t;
        }

        double T(int k, int i, int n)
        {
            if (k == 0)
            {
                double t1 = Math.PI * (4 * i - 1);
                double t2 = 4 * n + 2;
                return Math.Cos(t1 / t2);
            }
            double tprev = T(k - 1, i, n);
            return tprev - P(n, tprev) / PD(n, tprev);
        }

        double P(double n, double t)
        {
            return n switch
            {
                0 => 1,
                1 => t,
                _ => (2 * (n - 1) + 1) / ((n - 1) + 1) * t * P(n - 1, t)
                     - (n - 1) / ((n - 1) + 1) * P(n - 2, t)
            };
        }

        double PD(double n, double t)
        {
            double left = n / (1 - t * t);
            double right = P(n - 1, t) - t * P(n, t);
            return left * right;
        }
    }
}
