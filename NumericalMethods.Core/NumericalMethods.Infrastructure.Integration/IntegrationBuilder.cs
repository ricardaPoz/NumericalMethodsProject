using NumericalMethods.Infrastructure.Integration.Interfaces;
using NumericalMethods.Infrastructure.Integration.Methods.Parabolic;
using NumericalMethods.Infrastructure.Integration.Methods.Rectangle;
using NumericalMethods.Infrastructure.Integration.Methods.Splyne;
using NumericalMethods.Infrastructure.Integration.Methods.Trapezoid;
using NumericalMethods.Infrastructure.Integration.Methods.MonteCarlo;
using NumericalMethods.Infrastructure.Integration.Methods.Gauss;
using NumericalMethods.Infrastructure.Integration.Methods.Chebyshev;

namespace NumericalMethods.Infrastructure.Integration;
public record class IntegrationBuilder
{
	public IIntegratorWithConstantStep Build(string function, IntegrationMethodsWithConstantStep method)
	{
		return method switch
		{
			IntegrationMethodsWithConstantStep.Rectangle => new IntegratorWithConstantStep(new RectangleIntegrationMethod(), function),
			IntegrationMethodsWithConstantStep.Trapeze => new IntegratorWithConstantStep(new TrapezoidIntegrationMethod(), function),
			IntegrationMethodsWithConstantStep.Parabolic => new IntegratorWithConstantStep(new ParabolicIntegrationMethod(), function),
			IntegrationMethodsWithConstantStep.Spline => new IntegratorWithConstantStep(new SplyneIntegrationMethod(), function),
			_ => new IntegratorWithConstantStep(new SplyneIntegrationMethod(), function)
		};
	}

	public IIntegratorWithVariableStep Build(string function, IntegrationMethodsWithVariableStep method)
	{
		return method switch
		{
			IntegrationMethodsWithVariableStep.Gauss => new IntegratorWithVariableStep(new GaussIntegrationMethod(),function),
			IntegrationMethodsWithVariableStep.Chebyshev => new IntegratorWithVariableStep(new ChebyshevIntegrationMethod(), function),
			_ => throw new NotImplementedException()
		};
	}
	public IIntegratorMonteCarloMethod BuildMonteCarlo(string function)
    {
		return new IntegratorMonteCarloMethod(new MonteCarloIntegrationMethod(), function);
    }
}
