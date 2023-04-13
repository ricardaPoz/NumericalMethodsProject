namespace NumericalMethods.Infrastructure.Integration.Interfaces
{
    public interface IIntegratorMonteCarloMethod
    {
        public double Integrate(double start, double end, int count_points);
    }
}
