using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Interfaces;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.Newton;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.SimpleIterations;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.Seidel;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.ModifiedNewton;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.Secant;


namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems;
public record class NonLinearEquationsSolverBuilder()
{
	public INonLinearEquationsSystemsSolver Build(SolvingMethods SolvingMethod)
	{
		return SolvingMethod switch
		{
			SolvingMethods.Newton => new NonLinearEquationsSystemsSolver(new NewtonMethod()),
			SolvingMethods.SimpleIterations => new NonLinearEquationsSystemsSolver(new SimpleIterationsMethod()),
			SolvingMethods.Seidel => new NonLinearEquationsSystemsSolver(new SeidelMethod()),
			SolvingMethods.ModifiedNewton => new NonLinearEquationsSystemsSolver(new ModifiedNewtonMethod()),
			SolvingMethods.Secant => new NonLinearEquationsSystemsSolver(new SecantMethod()),
			_ => throw new NotImplementedException()
		};
	}
}
