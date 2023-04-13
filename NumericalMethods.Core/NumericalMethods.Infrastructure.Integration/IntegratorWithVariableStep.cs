using NumericalMethods.Infrastructure.Integration.Interfaces;
using NumericalMethods.Infrastructure.Integration.Methods;

namespace NumericalMethods.Infrastructure.Integration;
internal record class IntegratorWithVariableStep(IIntegrationMethodWithVariableStep IntegrationMethod, string Function) : IIntegratorWithVariableStep
{
	public double Integrate(double start, double end, int count_nodes)
	{
		return IntegrationMethod.Integrate(Function, start, end, count_nodes);
	}
}
