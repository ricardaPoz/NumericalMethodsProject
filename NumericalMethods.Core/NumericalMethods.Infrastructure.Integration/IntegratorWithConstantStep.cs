using NumericalMethods.Infrastructure.Integration.Interfaces;
using NumericalMethods.Infrastructure.Integration.Methods;

namespace NumericalMethods.Infrastructure.Integration;
internal record class IntegratorWithConstantStep(IIntegrationMethodWithConstantStep IntegrationMethod, string Function) : IIntegratorWithConstantStep
{
	public double Integrate(double start, double end, double step)
	{
		return IntegrationMethod.Integrate(Function, start, end, step);
	}
}