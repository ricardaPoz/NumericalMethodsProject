using NumericalMethods.Core.CauchyProblem.Interfaces;
using NumericalMethods.Core.CauchyProblem.Methods;

namespace NumericalMethods.Core.CauchyProblem;
public static class CauchyProblemBuilder
{
    public static ICauchyProblemSolver BuildWithOneStep(string function, OneStepMethods methodType)
    {
        return new CauchyProblemSolverWithOneStepMethod(GetChoice(methodType), function);
    }

    public static ICauchyProblemSolver BuildWithMultiStep(string function, MultiStepMethods multiStepMethodType, OneStepMethods oneStepMethodType, int preCalculatedPointsNumber)
    {
        IOneStepMethod one_step_method = GetChoice(oneStepMethodType);
        return multiStepMethodType switch
        {
            MultiStepMethods.AdamsBashforth => new CauchyProblemSolverWithMultiStepMethod(new AdamsBashforthMethod(), one_step_method, preCalculatedPointsNumber, function),
            MultiStepMethods.AdamsMoulton => new CauchyProblemSolverWithMultiStepMethod(new AdamsMoultonMethod(), one_step_method, preCalculatedPointsNumber, function)
        };
    }

    public static ICauchyProblemSolver CreateAdams(string function, OneStepMethods oneStepMethodType)
    {
        IOneStepMethod one_step_method = GetChoice(oneStepMethodType);
        return new CauchyProblemSolverWithMultiStepMethod(new AdamsMethod(), one_step_method, 4, function);
    }

    private static IOneStepMethod GetChoice(OneStepMethods methodType)
    {
        return methodType switch
        {
            OneStepMethods.Euler => new EulerMethod(),
            OneStepMethods.EulerIterative => new EulerIterativeMethod(),
            OneStepMethods.EulerImproved => new EulerImprovedMethod(),
            OneStepMethods.EulerRecalculation => new EulerRecalculationMethod(),
            OneStepMethods.RungeKuttaThridOrder => new RungeKuttaThridOrderMethod(),
            OneStepMethods.RungeKuttaFourthOrder => new RungeKuttaFourthOrderMethod()
        };
    }
}
