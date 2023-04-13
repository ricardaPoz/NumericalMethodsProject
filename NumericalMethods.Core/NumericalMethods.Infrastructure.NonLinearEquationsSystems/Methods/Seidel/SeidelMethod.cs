﻿using MathNet.Symbolics;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Shared;

namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods.Seidel
{
    internal class SeidelMethod : ISolvingMethod
        {
        private List<SymbolicExpression> functionExpressions;
        private double eps;
        private Dictionary<string, FloatingPoint> currentValues;
        private List<double> differences;

        public IEnumerable<IEnumerable<double>> SolveWithSteps(NonLinearEquationsSystem system, double eps, Dictionary<string, FloatingPoint> initialGuess)
        {
            Dictionary<string, FloatingPoint> sortedInitialGuess = initialGuess.OrderBy(x => x.Key).ToDictionary(el => el.Key, el => el.Value);

            if (Math.Abs(SquareMatrix.CreateJacobiMatrix(system.FunctionExpressions, initialGuess).GetNorm()) >= 1)
                throw new Exception("Данное приближение не подходит под условие сходимости попробуйте дургое");//Заменить message

            this.currentValues = sortedInitialGuess;
            this.functionExpressions = system.FunctionExpressions.ToList();
            this.eps = eps;

            differences = new List<double>() { double.MinValue };

            List<List<double>> results = (List<List<double>>)SolveRecursive(new List<List<double>>() { sortedInitialGuess.Select(el => el.Value.RealValue).ToList() });

            //Здесь добавляются разности
            for (int i = 0; i < results.Count; i++)
            {
                results[i].Add(differences[i]);
            }

            return results;
        }
        private IEnumerable<IEnumerable<double>> SolveRecursive(List<List<double>> results)
        {
            for (int i = 0; i < results.Last().Count; i++)
                currentValues[currentValues.ElementAt(i).Key] = results.Last()[i];

            //Считаем следующие значения x-ов
            results.Add(new List<double>());
            int countFunctionExpressions = functionExpressions.Count();
            for (int i = 0; i < countFunctionExpressions; i++)
            {
                results.Last().Add(functionExpressions.ElementAt(i).Evaluate(currentValues).RealValue);
                currentValues[currentValues.ElementAt(i).Key] = results.Last().Last();
            }

            differences.Add(FindMaxDifference(results.ElementAt(results.Count - 1), results.ElementAt(results.Count - 2)));
            if (differences.Last() > eps)
            {
                results = (List<List<double>>)SolveRecursive(results);
            }
            return results;
        }
        private double FindMaxDifference(List<double> currentValues, List<double> previousValues)
        {
            return currentValues.Zip(previousValues, (currentValue, previousValue) =>
            {
                return currentValue - previousValue;
            }).Max();
        }
    }
}
