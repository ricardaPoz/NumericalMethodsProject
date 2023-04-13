namespace NumericalMethods.Infrastructure.Integration.Methods
{
    internal interface IIntegrationMonteCarloMethod
    {
        public double Integrate(string function, double start, double end, int count_points);
    }
}
