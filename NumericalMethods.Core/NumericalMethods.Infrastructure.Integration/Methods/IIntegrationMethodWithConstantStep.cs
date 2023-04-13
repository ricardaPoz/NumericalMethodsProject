using NumericalMethods.Infrastructure.Integration.Interfaces;

namespace NumericalMethods.Infrastructure.Integration.Methods;
internal interface IIntegrationMethodWithConstantStep
{
	public double Integrate(string function, double start, double end, double step);
}
