using NumericalMethods.Infrastructure.Integration.Interfaces;
using NumericalMethods.Infrastructure.Integration.Methods;

namespace NumericalMethods.Infrastructure.Integration
{
    internal record class IntegratorMonteCarloMethod(IIntegrationMonteCarloMethod IntegrationMethod, string Function) : IIntegratorMonteCarloMethod
    {
        public double Integrate(double start, double end, int count_points)
        {
            return IntegrationMethod.Integrate(Function, start, end, count_points);
        }
    }
}
