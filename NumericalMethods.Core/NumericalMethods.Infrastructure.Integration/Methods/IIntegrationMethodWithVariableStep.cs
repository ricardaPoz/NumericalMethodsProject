using NumericalMethods.Infrastructure.Integration.Interfaces;

namespace NumericalMethods.Infrastructure.Integration.Methods;
internal interface IIntegrationMethodWithVariableStep
{
	public double Integrate(string function, double start, double end, int count_nodes);
}