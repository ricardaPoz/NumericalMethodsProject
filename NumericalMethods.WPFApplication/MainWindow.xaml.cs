using NumericalMethods.Core.Approximation;
using NumericalMethods.Core.Approximation.Interfaces;
using NumericalMethods.Core.Differentiation;
using NumericalMethods.Core.Differentiation.DifferentiationFunctions.UndefinedCoefficients;
using NumericalMethods.Core.Differentiation.Interfaces;
using NumericalMethods.Infrastructure.Integration;
using NumericalMethods.Infrastructure.Integration.Interfaces;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems;
using NumericalMethods.Core.CauchyProblem;
using NumericalMethods.Infrastructure.NonLinearEquationsSystems.Methods;

using org.mariuszgromada.math.mxparser;

using ScottPlot;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Expression = org.mariuszgromada.math.mxparser.Expression;
using Point = NumericalMethods.WPFApplication.Differentiation.Point;

namespace NumericalMethods.WPFApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		private readonly List<Point> _points = new List<Point>();
		private Function _current_function;
		private double _start_x;
		private double _end_x;
		private double _step;
		private int _order;
		private ResultTable _resultTable;
		public MainWindow()
		{
            InitializeComponent();

			Width = 1300;
			Height = 750;

			InitializeDifferentiation();
			InitializeIntegration();
			InitializeNonLinerEquation();
			InitializeCauchyProblem();

		}

		#region DifferentiationTab
		private void InitializeDifferentiation()
		{
			foreach (var item in Enum.GetValues(typeof(InterpolationFunctionType)))
			{
				Differentiation_InterpolationFunctionTypeComboBox.Items.Add(item.ToString());
			}

			Differentiation_InterpolationFunctionTypeComboBox.SelectedItem = Differentiation_InterpolationFunctionTypeComboBox.Items[0];

			foreach (var item in Enum.GetValues(typeof(DifferentiationFunctionType)))
			{
				Differentiation_FunctionTypeComboBox.Items.Add(item.ToString());
			}

			Differentiation_FunctionTypeComboBox.SelectedItem = Differentiation_InterpolationFunctionTypeComboBox.Items[0];

			Differentiation_FunctionTextBox.Text = "x^2";
			Differentiation_StartXTextBox.Text = "-3";
			Differentiation_EndXTextBox.Text = "3";
			Differentiation_StepTextBox.Text = "1";
			Differentiation_NumberOfMembers.Text = "2";
			DerivativeDegreeTextBox.Text = "1";

			Differentiation_MainChart.Plot.Legend(enable: true);
		}
		private void Differentiation_AddNodesOnChart_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(Differentiation_FunctionTextBox.Text) ||
				String.IsNullOrEmpty(Differentiation_StartXTextBox.Text) ||
				String.IsNullOrEmpty(Differentiation_EndXTextBox.Text) ||
				String.IsNullOrEmpty(Differentiation_StepTextBox.Text))
			{
				MessageBox.Show("Для начала рассчёта, заполните все поля");
				return;
			}
			_points.Clear();
			_current_function = new Function("f(x) = " + Differentiation_FunctionTextBox.Text.Trim());
			double start_x = new Expression(Differentiation_StartXTextBox.Text.Trim()).calculate();
			double end_x = new Expression(Differentiation_EndXTextBox.Text.Trim()).calculate();
			double step = new Expression(Differentiation_StepTextBox.Text.Trim()).calculate();
			int roundNumbers = step.ToString().Contains(',') ? step.ToString().Split(',')[1].Length % 16 : 1;
			for (double x = start_x; Math.Round(x, roundNumbers) <= end_x; x += step)
			{
				double y = _current_function.calculate(x);
				_points.Add(new Point(x, y));
			}

			double[] xs = _points.Select(node => node.X).ToArray();
			double[] ys = _points.Select(node => node.Y).ToArray();
			Differentiation_MainChart.Plot.AddScatter(xs, ys, lineWidth: 0, markerShape: MarkerShape.filledCircle, color: System.Drawing.Color.Red, markerSize: 7, label: "nodes");
			Differentiation_MainChart.Refresh();
		}
		private void Differentiation_FunctionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Differentiation_AccuracyGrid.Visibility = Visibility.Collapsed;
			Differentiation_NumberOfMembersGrid.Visibility = Visibility.Collapsed;
			Differentiation_Accuracy.Text = "";
			Differentiation_NumberOfMembers.Text = "";
			DifferentiationFunctionType differentiationFunctionType = (DifferentiationFunctionType)Enum.Parse(typeof(DifferentiationFunctionType), ((ComboBox)sender).SelectedValue.ToString());
			if(differentiationFunctionType == DifferentiationFunctionType.NewtonPolynomials)
            {
				Differentiation_NumberOfMembersGrid.Visibility = Visibility.Visible;

			}
			if (differentiationFunctionType is DifferentiationFunctionType.Runge or DifferentiationFunctionType.UndefinedCoefficients)
            {
				Differentiation_AccuracyGrid.Visibility = Visibility.Visible;
			}
		}
		private void Differentiation_AddOnChartInterpolationButton_Click(object sender, RoutedEventArgs e)
		{
			if(_points.Count == 0)
            {
				MessageBox.Show("Добавьте точки");
				return;
            }
			string? function_type_string = Differentiation_InterpolationFunctionTypeComboBox.SelectedValue.ToString();
			if (function_type_string is null) return;
			InterpolationFunctionType interpolation_type = Enum.Parse<InterpolationFunctionType>(function_type_string);
			IInterpolationFunction? interpolation_function = InterpolationBuilder.Build(_points, interpolation_type);
			if (interpolation_function is null) return;
			var xs = new List<double>();
			var ys = new List<double>();
			double start_x = new Expression(Differentiation_StartXTextBox.Text.Trim()).calculate();
			double end_x = new Expression(Differentiation_EndXTextBox.Text.Trim()).calculate();
			double step = new Expression(Differentiation_StepTextBox.Text.Trim()).calculate();
			int roundNumbers = step.ToString().Contains(',') ? step.ToString().Split(',')[1].Length % 16 : 1;
			for (double x = start_x; Math.Round(x, roundNumbers) <= end_x; x += step)
			{
				var y = interpolation_function.Calculate(x);
				if (y is null) continue;

				xs.Add(x);
				ys.Add((double)y);
			}
			Differentiation_MainChart.Plot.AddScatter(xs.ToArray(), ys.ToArray(), lineWidth: 4, markerSize: 0, label: "interpolation");
			Differentiation_MainChart.Refresh();
			Differentiation_MainChart.Refresh();
		}
		private void Differentiation_AddOnChartButton_Click(object sender, RoutedEventArgs e)
		{
			if (_points.Count == 0)
			{
				MessageBox.Show("Добавьте точки");
				return;
			}
			var xs = new List<double>();
			var ys = new List<double>();
			_start_x = new Expression(Differentiation_StartXTextBox.Text.Trim()).calculate();
			_end_x = new Expression(Differentiation_EndXTextBox.Text.Trim()).calculate();
			_step = new Expression(Differentiation_StepTextBox.Text.Trim()).calculate();
			int roundNumbers = _step.ToString().Contains(',') ? _step.ToString().Split(',')[1].Length % 16 : 1;
			int derivative_degree = int.Parse(DerivativeDegreeTextBox.Text.Trim());
			string? function_type_string = Differentiation_FunctionTypeComboBox.SelectedValue.ToString();
			if (function_type_string is null) return;

			DifferentiationFunctionType interpolation_type = (DifferentiationFunctionType)Enum.Parse(typeof(DifferentiationFunctionType), function_type_string);

            switch (interpolation_type)
            {
				case DifferentiationFunctionType.NewtonPolynomials:
                    if (String.IsNullOrEmpty(Differentiation_NumberOfMembers.Text))
                    {
						MessageBox.Show("Введите number of members");
						return;
                    }
					int numberOfMembers = int.Parse(Differentiation_NumberOfMembers.Text);
					INewtonDifferentiationFunction newton_function = DifferentiationBuilder.CreateNewton(_points, _step, derivative_degree, numberOfMembers);
					for (double x = _start_x; Math.Round(x, roundNumbers) <= _end_x; x += _step)
					{
                        double? y = newton_function.Calculate(x);
						if (y is null) continue;

						xs.Add(x);
						ys.Add((double)y);
					}
					break;
				case DifferentiationFunctionType.UndefinedCoefficients:
                    if (String.IsNullOrEmpty(Differentiation_Accuracy.Text))
                    {
						MessageBox.Show("Введите accuracy");
						return;
                    }
					int count_coefficients_c = int.Parse(Differentiation_Accuracy.Text);
					List<Point> additional_right_points = new List<Point>();
                    for (int i = 1; i <= (count_coefficients_c - 2) * derivative_degree; i++)
                    {
						double new_x = _end_x + _step * i;
						double new_y = _current_function.calculate(new_x);
						Point additional_point = new Point(new_x, new_y);
						additional_right_points.Add(additional_point);
                    }

					List<Point> additional_left_point = new List<Point>();
                    for (int i = derivative_degree; i >= 1; i--)
                    {
						double new_x = _start_x - _step * i;
						double new_y = _current_function.calculate(new_x);
						Point additional_point = new Point(new_x, new_y);
						additional_left_point.Add(additional_point);
					}
					IEnumerable<Point> new_points = additional_left_point.Concat(_points.Concat(additional_right_points));
					IUndefinedCoefficientsDifferentiationFunction undefined_coefficients_function = DifferentiationBuilder.CreateUndefinedCoefficients(new_points, _step, derivative_degree, count_coefficients_c);
                    IEnumerable<IDifferentiationResultNode> result = undefined_coefficients_function.Calculate();
					xs = result.Select(node => node.X).ToList();
					ys = result.Select(node => node.Y).ToList();
					break;
				case DifferentiationFunctionType.Runge:
                    {
						if (String.IsNullOrEmpty(Differentiation_Accuracy.Text))
						{
							MessageBox.Show("Введите accuracy");
							return;
						}
						IDifferentiationFunction differentiation_function = DifferentiationBuilder.CreateRunge(_points, _step, derivative_degree, 1, 2);
						for (double x = _start_x; Math.Round(x, roundNumbers) <= _end_x; x += _step)
						{
							double? y = differentiation_function.Calculate(x);
							if (y is null) continue;

							xs.Add(x);
							ys.Add((double)y);
						}
						break;
					}
				default:
                    {
						IDifferentiationFunction differentiation_function = DifferentiationBuilder.Build(_points, interpolation_type, _step, derivative_degree);
						for (double x = _start_x; Math.Round(x, roundNumbers) <= _end_x; x += _step)
						{
							double? y = differentiation_function?.Calculate(x);
							if (y is null) continue;

							xs.Add(x);
							ys.Add((double)y);
						}
						break;
					}
			}
			
			Differentiation_MainChart.Plot.AddScatter(xs.ToArray(), ys.ToArray(), lineWidth: 4, markerSize: 0, label: "differentiation");
			Differentiation_MainChart.Refresh();
		}
		private void Differentiation_ClearChartButton_Click(object sender, RoutedEventArgs e)
		{
			Differentiation_MainChart.Plot.Clear();
			Differentiation_MainChart.Refresh();
		}
		#endregion

		#region IntegrationTab
		private void InitializeIntegration()
		{
			foreach (var item in Enum.GetValues(typeof(IntegrationMethodsWithConstantStep)))
			{
				Integration_MethodComboBox.Items.Add(item.ToString());
			}
			foreach(var item in Enum.GetValues(typeof(IntegrationMethodsWithVariableStep)))
            {
				Integration_MethodComboBox.Items.Add(item.ToString());
			}
			Integration_MethodComboBox.Items.Add("MonteCarlo");

			Integration_MethodComboBox.SelectedItem = Integration_MethodComboBox.Items[0];

            Integration_FunctionTextBox.Text = "x^2";
            Integration_StartXTextBox.Text = "-5";
            Integration_EndXTextBox.Text = "5";
            Integration_StepTextBox.Text = "0.1";

            Integration_MainChart.Plot.Legend(enable: true);
		}
		private void Integration_MethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Enum.IsDefined(typeof(IntegrationMethodsWithConstantStep), ((ComboBox)sender).SelectedValue))
			{
				Integration_CountNodesGrid.Visibility = Visibility.Collapsed;
				Integration_CountPointGrid.Visibility = Visibility.Collapsed;
			}
			if (Enum.IsDefined(typeof(IntegrationMethodsWithVariableStep), ((ComboBox)sender).SelectedValue))
			{
				Integration_CountNodesGrid.Visibility = Visibility.Visible;
				Integration_CountPointGrid.Visibility = Visibility.Collapsed;
			}
			if (((ComboBox)sender).SelectedValue.ToString() == "MonteCarlo")
			{
				Integration_CountNodesGrid.Visibility = Visibility.Collapsed; 
				Integration_CountPointGrid.Visibility = Visibility.Visible;
			}
			Integration_CountPointTextBox.Text = "";
			Integration_CountNodesTextBox.Text = "";
		}
		private void Integration_CalculateButton_Click(object sender, RoutedEventArgs e)
		{
			Integration_MainChart.Plot.Clear();
			if (String.IsNullOrEmpty(Integration_FunctionTextBox.Text)||
				String.IsNullOrEmpty(Integration_StartXTextBox.Text)||
				String.IsNullOrEmpty(Integration_EndXTextBox.Text)||
				String.IsNullOrEmpty(Integration_StepTextBox.Text))
            {
				MessageBox.Show("Для начала рассчёта, заполните все поля");
				return;
            }
			Function function = new Function("f(x) = " + Integration_FunctionTextBox.Text.Trim());
			double start_x = new Expression(Integration_StartXTextBox.Text.Trim()).calculate();
			double end_x = new Expression(Integration_EndXTextBox.Text.Trim()).calculate();
			double step = new Expression(Integration_StepTextBox.Text.Trim()).calculate();

			string function_type_string = Integration_MethodComboBox.SelectedValue.ToString();
			double integration_result = 0;
			if (Enum.IsDefined(typeof(IntegrationMethodsWithConstantStep), function_type_string))
			{
				IntegrationMethodsWithConstantStep method = (IntegrationMethodsWithConstantStep)Enum.Parse(typeof(IntegrationMethodsWithConstantStep), function_type_string);
				IIntegratorWithConstantStep integrator = new IntegrationBuilder().Build(Integration_FunctionTextBox.Text.Trim(), method);
				integration_result = integrator.Integrate(start_x, end_x, step);
			}
			if (Enum.IsDefined(typeof(IntegrationMethodsWithVariableStep), function_type_string))
			{
                if (String.IsNullOrEmpty(Integration_CountNodesTextBox.Text))
                {
					MessageBox.Show("Для начала рассчёта, заполните поле count nodes");
					return;
                }
				int countNodes = int.Parse(Integration_CountNodesTextBox.Text);
				IntegrationMethodsWithVariableStep method = (IntegrationMethodsWithVariableStep)Enum.Parse(typeof(IntegrationMethodsWithVariableStep), function_type_string);
				IIntegratorWithVariableStep integrator = new IntegrationBuilder().Build(Integration_FunctionTextBox.Text.Trim(), method);
				integration_result = integrator.Integrate(start_x, end_x, countNodes);
			}
			if (function_type_string == "MonteCarlo")
			{
				if (String.IsNullOrEmpty(Integration_CountPointTextBox.Text))
				{
					MessageBox.Show("Для начала рассчёта, заполните поле count point");
					return;
				}
				int countPoints = int.Parse(Integration_CountPointTextBox.Text);
				IIntegratorMonteCarloMethod integrator = new IntegrationBuilder().BuildMonteCarlo(Integration_FunctionTextBox.Text.Trim());
				integration_result = integrator.Integrate(start_x, end_x, countPoints);
			}

			(double[] xs, double[] ys) = DataGenerate(function, start_x - 5, end_x + 5, step);

			Integration_MainChart.Plot.AddScatter(xs.ToArray(), ys.ToArray(), lineWidth: 4, markerSize: 0, label: "function");

			double[] interval_start_x = { start_x, start_x };
			double[] interval_start_y = { 0, function.calculate(start_x) };
			Integration_MainChart.Plot.AddScatter(interval_start_x, interval_start_y, lineWidth: 4, markerSize: 0, label: "start integrate");

			double[] interval_end_x = { end_x, end_x };
			double[] interval_end_y = { 0, function.calculate(end_x) };
			Integration_MainChart.Plot.AddScatter(interval_end_x, interval_end_y, lineWidth: 4, markerSize: 0, label: "end integrate");

			(double[] fill_xs, double[] fill_ys) = DataGenerate(function, start_x, end_x, step);

			Integration_MainChart.Plot.AddFill(fill_xs, fill_ys, baseline: 0, color: System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red));
			Integration_MainChart.Plot.AddHorizontalLine(0, color: System.Drawing.Color.Black);
			Integration_MainChart.Refresh();

            MessageBox.Show($"Integration result: {integration_result}");
        }
		#endregion

		#region Tools
		private static (double[] xs, double[] ys) DataGenerate(Function function, double start, double end, double step)
		{
			int size = (int)Math.Ceiling((end - start) / step);
			double[] xs = new double[size + 1];
			double[] ys = new double[size + 1];
			int i = 0;
			for (double x = start; x <= end; x += step, i++)
			{
				xs[i] = x;
				ys[i] = function.calculate(x);
			}

			return (xs, ys);
		}
		#endregion
		#region SystemOfNonLinearEquationsTab
		private void InitializeNonLinerEquation()
		{
			Enum.GetNames(typeof(SolvingMethods)).ToList().ForEach(nameSolvingMethod =>
			{
				SNE_methodComboBox.Items.Add(nameSolvingMethod);
			});
			SNE_methodComboBox.SelectedItem = SNE_methodComboBox.Items[0];
			if (SNE_systemOfEquationsGrid.RowDefinitions.Count == 0)
				SNE_RemoveEquationButton.IsEnabled = false;
		}
		private void InitialDefaulSystemOfEquations()
		{
			Canvas SNE_equationsCanvas;
			int start_position = 10;
			if (SNE_systemOfEquationsGrid.RowDefinitions.Count < 1)
				start_position = 10;
			else
				start_position += 40 * SNE_systemOfEquationsGrid.RowDefinitions.Count;

			RowDefinition rowDefinition = new RowDefinition();
			rowDefinition.Height = new GridLength(52, GridUnitType.Pixel);
			SNE_equationsCanvas = new Canvas();
			SNE_systemOfEquationsGrid.RowDefinitions.Add(rowDefinition);
			SNE_systemOfEquationsGrid.Children.Add(SNE_equationsCanvas);

			Label equationLabel = new Label();
			equationLabel.Content = "f(x) = ";
			equationLabel.FontSize = 14;
			equationLabel.Margin = new Thickness(10, start_position, 0, 0);
			SNE_equationsCanvas.Children.Add(equationLabel);

			TextBox equationOfSystem = new TextBox();
			equationOfSystem.Margin = new Thickness(60, start_position, 20, 0);
			equationOfSystem.FontSize = 14;
			equationOfSystem.Width = 200;
			equationOfSystem.Height = 30;
			SNE_equationsCanvas.Children.Add(equationOfSystem);
		}
		private void InitialOtherSystemOfEquations()
		{
			Canvas SNE_equationsCanvas;
			int start_position = 10;
			if (SNE_systemOfEquationsGrid.RowDefinitions.Count < 1)
				start_position = 10;
			else
				start_position += 40 * SNE_systemOfEquationsGrid.RowDefinitions.Count;

			RowDefinition rowDefinition = new RowDefinition();
			rowDefinition.Height = new GridLength(52, GridUnitType.Pixel);
			SNE_equationsCanvas = new Canvas();
			SNE_systemOfEquationsGrid.RowDefinitions.Add(rowDefinition);
			SNE_systemOfEquationsGrid.Children.Add(SNE_equationsCanvas);

			Label equationLabel = new Label();
			equationLabel.Content = $"x{SNE_systemOfEquationsGrid.RowDefinitions.Count} = ";
			equationLabel.FontSize = 14;
			equationLabel.Margin = new Thickness(10, start_position, 0, 0);
			SNE_equationsCanvas.Children.Add(equationLabel);

			TextBox equationOfSystem = new TextBox();
			equationOfSystem.Margin = new Thickness(60, start_position, 20, 0);
			equationOfSystem.FontSize = 14;
			equationOfSystem.Width = 200;
			equationOfSystem.Height = 30;
			SNE_equationsCanvas.Children.Add(equationOfSystem);
		}
		private void GenerateInitialApproximation()
		{
			Canvas SNE_initialApproximationCanvas;
			int start_position = 10;
			if (SNE_initialApproximationGrid.RowDefinitions.Count < 1)
				start_position = 10;
			else
				start_position += 40 * SNE_initialApproximationGrid.RowDefinitions.Count;

			RowDefinition rowDefinition = new RowDefinition();
			rowDefinition.Height = new GridLength(52, GridUnitType.Pixel);
			SNE_initialApproximationCanvas = new Canvas();
			SNE_initialApproximationGrid.RowDefinitions.Add(rowDefinition);
			SNE_initialApproximationGrid.Children.Add(SNE_initialApproximationCanvas);

			Label equationLabel = new Label();
			equationLabel.Content = $"x{SNE_systemOfEquationsGrid.RowDefinitions.Count}";
			equationLabel.FontSize = 14;
			equationLabel.Margin = new Thickness(10, start_position, 0, 0);
			SNE_initialApproximationCanvas.Children.Add(equationLabel);

			TextBox equationOfSystem = new TextBox();
			equationOfSystem.Margin = new Thickness(60, start_position, 20, 0);
			equationOfSystem.FontSize = 14;
			equationOfSystem.Width = 50;
			equationOfSystem.Height = 30;
			SNE_initialApproximationCanvas.Children.Add(equationOfSystem);
		}
		private void GenerateResultTable(IEnumerable<IEnumerable<double>> result, IEnumerable<string> variablesNames, SolvingMethods solvingMethod)
		{
			variablesNames = variablesNames.OrderBy(el => el);
			DataTable dataTable = new DataTable();
			SNE_ResultDataGrid.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);
			dataTable.Columns.Add("k");
			foreach (var variableName in variablesNames)
			{
				dataTable.Columns.Add($"{variableName}(k)");
			}
			dataTable.Columns.Add(solvingMethod == SolvingMethods.Secant ? "Sk" : "Difference");
			int countResult = result.Count();
			for (int i = 0; i < countResult; i++)
			{
				object[] row = new object[variablesNames.Count() + 2];
				row[0] = i;
				for (int j = 0; j < result.ElementAt(i).Count() - 1; j++)
				{
					row[j + 1] = string.Format("{0:F4}", result.ElementAt(i).ElementAt(j));
				}
				row[^1] = result.ElementAt(i).Last() == double.MinValue ? "-" : string.Format("{0:F4}", result.ElementAt(i).Last());
				dataTable.Rows.Add(row);
			}
			SNE_ResultDataGrid.ItemsSource = dataTable.AsDataView();
		}
		private void SNE_AddEquationButton_Click(object sender, RoutedEventArgs e)
		{
			SNE_ResultDataGrid.ItemsSource = null;
			string method_type = SNE_methodComboBox.SelectedItem.ToString();
			if (method_type == SolvingMethods.Seidel.ToString() || method_type == SolvingMethods.SimpleIterations.ToString()) { InitialOtherSystemOfEquations(); }
			else { InitialDefaulSystemOfEquations(); }
			GenerateInitialApproximation();
			SNE_RemoveEquationButton.IsEnabled = true;
		}
		private void SNE_RemoveEquationButton_Click(object sender, RoutedEventArgs e)
		{
			SNE_systemOfEquationsGrid.RowDefinitions.Remove(SNE_systemOfEquationsGrid.RowDefinitions.Last());
			SNE_systemOfEquationsGrid.Children.Remove(SNE_systemOfEquationsGrid.Children[^1]);
			SNE_initialApproximationGrid.RowDefinitions.Remove(SNE_initialApproximationGrid.RowDefinitions.Last());
			SNE_initialApproximationGrid.Children.Remove(SNE_initialApproximationGrid.Children[^1]);
			if (SNE_systemOfEquationsGrid.RowDefinitions.Count < 1)
				SNE_RemoveEquationButton.IsEnabled = false;
		}
		private void SNE_CalculateButton_Click(object sender, RoutedEventArgs e)
		{
			if (SNE_systemOfEquationsGrid.Children.Count == 0 || SNE_initialApproximationGrid.Children.Count == 0)
			{
				MessageBox.Show("Добавьте уравнения и начальное приближение");
				return;
			}
			List<string> functions = new List<string>();
			foreach (var childrenGrid in SNE_systemOfEquationsGrid.Children)
			{
				Canvas? canvas = childrenGrid as Canvas;
				foreach (object? childrenCanvas in canvas.Children)
				{
					if (childrenCanvas is TextBox)
					{
						TextBox textBox = childrenCanvas as TextBox;
						if (String.IsNullOrEmpty(textBox.Text))
						{
							MessageBox.Show("Заполните все поля, связанные с функциями");
							return;
						}
						functions.Add(textBox.Text.Trim());
					}
				}
			}
			Dictionary<string, MathNet.Symbolics.FloatingPoint> initialGuess = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
			string initialGuessKey = null;
			MathNet.Symbolics.FloatingPoint initialGuessValue = null;
			foreach (var childrenGrid in SNE_initialApproximationGrid.Children)
			{
				Canvas canvas = childrenGrid as Canvas;
				foreach (var childrenCanvas in canvas.Children)
				{
					if (childrenCanvas is Label)
					{
						Label label = childrenCanvas as Label;
						initialGuessKey = label.Content.ToString();
					}
					if (childrenCanvas is TextBox)
					{
						TextBox textBox = childrenCanvas as TextBox;
						if (String.IsNullOrEmpty(textBox.Text))
						{
							MessageBox.Show("Заполните все поля, связанные с начальным приближением");
							return;
						}
						initialGuessValue = double.Parse(textBox.Text);
					}
				}
				initialGuess.Add(initialGuessKey, initialGuessValue);
			}
			if (String.IsNullOrEmpty(SNE_AccuracyTextBox.Text))
			{
				MessageBox.Show("Заполните поле точность");
				return;
			}
			IEnumerable<IEnumerable<double>> res;
			try
			{
				res = new NonLinearEquationsSolverBuilder().Build((SolvingMethods)Enum.Parse(typeof(SolvingMethods), SNE_methodComboBox.SelectedValue.ToString())).SolveWithSteps(new NonLinearEquationsSystem(functions), double.Parse(SNE_AccuracyTextBox.Text), initialGuess);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return;
			}
			res = res.Select(row => row.Select(item => Math.Round(item, 4)));
			GenerateResultTable(res, initialGuess.Select(el => el.Key), (SolvingMethods)Enum.Parse(typeof(SolvingMethods), SNE_methodComboBox.SelectedValue.ToString()));
		}

		#endregion

		#region CauchyProblem
		private void InitializeCauchyProblem()
        {
            foreach (var item in Enum.GetValues(typeof(OneStepMethods)))
            {
				CauchyProblem_FunctionTypeComboBox.Items.Add(item);
			}
            foreach (var item in Enum.GetValues(typeof(MultiStepMethods)))
            {
				CauchyProblem_FunctionTypeComboBox.Items.Add(item);
			}
			CauchyProblem_FunctionTypeComboBox.Items.Add("Adams");
			foreach (var item in Enum.GetValues(typeof(InterpolationFunctionType)))
			{
				CauchyProblem_InterpolationFunctionTypeComboBox.Items.Add(item);
			}
			foreach (var item in Enum.GetValues(typeof(OneStepMethods)))
			{
				CauchyProblem_OneStepMethodComboBox.Items.Add(item);
			}
			CauchyProblem_InterpolationFunctionTypeComboBox.SelectedItem = CauchyProblem_InterpolationFunctionTypeComboBox.Items[0];
			CauchyProblem_FunctionTypeComboBox.SelectedItem = CauchyProblem_FunctionTypeComboBox.Items[0];
			CauchyProblem_OneStepMethodComboBox.SelectedItem = CauchyProblem_OneStepMethodComboBox.Items[^1];
			CauchyProblem_OrderTextBox.Text = "3";
			CauchyProblem_MaxOrderFunctionTextBox.Text = "-3*y2-3*y1-y0";
			CauchyProblem_EndXTextBox.Text = "10";
			CauchyProblem_StepTextBox.Text = "0,05";
			CauchyProblem_InitXTextBox.Text = "0";
			CauchyProblem_MainChart.Plot.Legend(enable: true);
		}
		private void CauchyProblem_OrderSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(CauchyProblem_OrderTextBox.Text))
            {
				MessageBox.Show("Заполните поле order");
				CauchyProblem_Properties.Visibility = Visibility.Collapsed;
				return;
				
			}
			_order = int.Parse(CauchyProblem_OrderTextBox.Text);
			CauchyProblem_FillPropertiesGrid();
			CauchyProblem_Properties.Visibility = Visibility.Visible;
		}
		private string FormingContenLabelOrderFunction(int order)
        {
			return order <= 3 ? new StringBuilder("Y").Append('\'', order).ToString() : $"Y^({order})";
		}
		private void CauchyProblem_FillPropertiesGrid()
        {
			CauchyProblem_MaxOrderFunctionLabel.Content = FormingContenLabelOrderFunction(_order);
		}
		private void CauchyProblem_EditLabelsInitFunctions(double x)
        {
			foreach(var children in CauchyProblem_InitFunctionsGrid.Children)
            {
				if(children is Grid)
                {
					foreach(var gridChildren in ((Grid)children).Children)
                    {
						if(gridChildren is Label)
                        {
							string content = ((Label)gridChildren).Content.ToString();
							int f = content.IndexOf('(');
							((Label)gridChildren).Content = content.Remove(content.LastIndexOf('(')) + $"({x})";
							break;
						}
                    }
                }
            }
		}
		private void CauchyProblem_FunctionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(((ComboBox)sender).SelectedValue.ToString() == "Adams")
            {
				CauchyProblem_PreCalcCountGrid.Visibility = Visibility.Collapsed;
				CauchyProblem_OneStepMethodComboBox.Visibility = Visibility.Visible;
				return;
			}
            if (Enum.IsDefined(typeof(MultiStepMethods), ((ComboBox)sender).SelectedValue.ToString())){
				CauchyProblem_OneStepMethodComboBox.Visibility = Visibility.Visible;
				CauchyProblem_PreCalcCountGrid.Visibility = Visibility.Visible;
			}
            else
            {
				CauchyProblem_OneStepMethodComboBox.Visibility = Visibility.Collapsed;
				CauchyProblem_PreCalcCountGrid.Visibility = Visibility.Collapsed;
			}
		}
		private void CauchyProblem_FillInitFunctionsGrid()
        {
			CauchyProblem_InitFunctionsGrid.RowDefinitions.Clear();
			CauchyProblem_InitFunctionsGrid.Children.Clear();
			Grid grid;
			Label label;
			TextBox textBox;
            for (int i = 0; i < _order; i++)
            {
				CauchyProblem_InitFunctionsGrid.RowDefinitions.Add(new RowDefinition());
				grid = new Grid()
				{
					Height = 30,
					Margin = new Thickness(0, 10, 0, 0)
				};
				grid.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width = new GridLength(50)
				});
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				label = new Label()
				{
					Content = FormingContenLabelOrderFunction(i) + "()"
				};
				textBox = new TextBox();
				grid.Children.Add(label);
				Grid.SetColumn(grid.Children[^1], 0);
				grid.Children.Add(textBox);
				Grid.SetColumn(grid.Children[^1], 1);
				CauchyProblem_InitFunctionsGrid.Children.Add(grid);
				Grid.SetRow(CauchyProblem_InitFunctionsGrid.Children[^1], i);

			}
        }
		private void CauchyProblem_InitXTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (String.IsNullOrEmpty(((TextBox)sender).Text))
			{
				CauchyProblem_ParentInitFunctionsGrid.Visibility = Visibility.Collapsed;
				CauchyProblem_InitFunctionsGrid.RowDefinitions.Clear();
				CauchyProblem_InitFunctionsGrid.Children.Clear();
			}
			else
			{
				if (((TextBox)sender).Text.Last() is '.' or '-') return;
				if (CauchyProblem_InitFunctionsGrid.Children.Count == 0)
				{
					CauchyProblem_FillInitFunctionsGrid();
					CauchyProblem_ParentInitFunctionsGrid.Visibility = Visibility.Visible;
				}
				CauchyProblem_EditLabelsInitFunctions(double.Parse(((TextBox)sender).Text));
			}
		}
		private void CauchyProblem_AddOnChartButton_Click(object sender, RoutedEventArgs e)
		{
			if(String.IsNullOrEmpty(CauchyProblem_OrderTextBox.Text)||
				String.IsNullOrEmpty(CauchyProblem_MaxOrderFunctionTextBox.Text)||
				String.IsNullOrEmpty(CauchyProblem_EndXTextBox.Text) ||
				String.IsNullOrEmpty(CauchyProblem_StepTextBox.Text)||
				String.IsNullOrEmpty(CauchyProblem_InitXTextBox.Text))
            {
				MessageBox.Show("заполните все поля");
				return;
            }
			if(CauchyProblem_InitFunctionsGrid.Children.Count == 0)
            {
				MessageBox.Show("заполните все поля");
				return;
			}
			foreach (var children in CauchyProblem_InitFunctionsGrid.Children)
			{
				if (children is Grid)
				{
					foreach (var gridChildren in ((Grid)children).Children)
					{
						if (gridChildren is TextBox)
						{
							if(String.IsNullOrEmpty((gridChildren as TextBox).Text))
                            {
								MessageBox.Show("заполните все поля");
								return;
							}
						}
					}
				}
			}
			Dictionary<string, double> initialGuess = CauchyProblem_CreateInitialGuess();
			if (CauchyProblem_FunctionTypeComboBox.SelectedValue.ToString() == "Adams")
			{
				_resultTable = CauchyProblemBuilder.CreateAdams(
				function: CauchyProblem_MaxOrderFunctionTextBox.Text,
				oneStepMethodType: Enum.Parse<OneStepMethods>(CauchyProblem_OneStepMethodComboBox.SelectedValue.ToString()))
				.Calculate(
					b: double.Parse(CauchyProblem_EndXTextBox.Text),
					h: double.Parse(CauchyProblem_StepTextBox.Text),
					initialGuess: (x: double.Parse(CauchyProblem_InitXTextBox.Text), ys: initialGuess)
				);
				return;
			}
			if (Enum.IsDefined(typeof(MultiStepMethods), CauchyProblem_FunctionTypeComboBox.SelectedValue.ToString()))
			{
				if (String.IsNullOrEmpty(CauchyProblem_PreCalcCount.Text))
                {
					MessageBox.Show("заполните все поля");
					return;
				}
				_resultTable = CauchyProblemBuilder.BuildWithMultiStep(
					function: CauchyProblem_MaxOrderFunctionTextBox.Text,
					multiStepMethodType: Enum.Parse<MultiStepMethods>(CauchyProblem_FunctionTypeComboBox.SelectedValue.ToString()),
					oneStepMethodType: Enum.Parse<OneStepMethods>(CauchyProblem_OneStepMethodComboBox.SelectedValue.ToString()),
					preCalculatedPointsNumber: int.Parse(CauchyProblem_PreCalcCount.Text))
					.Calculate(
						b: double.Parse(CauchyProblem_EndXTextBox.Text),
						h: double.Parse(CauchyProblem_StepTextBox.Text),
						initialGuess: (x: double.Parse(CauchyProblem_InitXTextBox.Text), ys: initialGuess)
					);
			}
			if(Enum.IsDefined(typeof(OneStepMethods), CauchyProblem_FunctionTypeComboBox.SelectedValue.ToString()))
			{
				_resultTable = CauchyProblemBuilder.BuildWithOneStep(
					function: CauchyProblem_MaxOrderFunctionTextBox.Text,
					methodType: Enum.Parse<OneStepMethods>(CauchyProblem_FunctionTypeComboBox.SelectedValue.ToString()))
					.Calculate(
						b: double.Parse(CauchyProblem_EndXTextBox.Text),
						h: double.Parse(CauchyProblem_StepTextBox.Text),
						initialGuess: (x: double.Parse(CauchyProblem_InitXTextBox.Text), ys: initialGuess)
					);
			}
			foreach(var y in initialGuess)
            {
				List<(double x, double yi)> points = _resultTable[y.Key];
				CauchyProblem_MainChart.Plot.AddScatter(points.Select(point=> point.x).ToArray(), points.Select(point => point.yi).ToArray(), lineWidth: 0, markerSize: 5, label: y.Key);
			}
			CauchyProblem_MainChart.Refresh();
		}
		private Dictionary<string, double> CauchyProblem_CreateInitialGuess()
        {
			Dictionary<string, double> result = new Dictionary<string, double>();
			for (int i = 0; i < CauchyProblem_InitFunctionsGrid.Children.Count; i++)
            {
				if (CauchyProblem_InitFunctionsGrid.Children[i] is Grid)
				{
					foreach (var gridChildren in ((Grid)CauchyProblem_InitFunctionsGrid.Children[i]).Children)
					{
						if (gridChildren is TextBox)
						{
							result.Add($"y{i}", double.Parse(((TextBox)gridChildren).Text));
							break;
						}
					}
				}
			}
			return result;
		}
        #endregion
        private void CauchyProblem_ClearChartButton_Click(object sender, RoutedEventArgs e)
        {
			CauchyProblem_MainChart.Plot.Clear();
			CauchyProblem_MainChart.Refresh();
		}
        private void CauchyProblem_AddOnChartInterpolationButton_Click(object sender, RoutedEventArgs e)
        {
			string? function_type_string = CauchyProblem_InterpolationFunctionTypeComboBox.SelectedValue.ToString();
			if (function_type_string is null) return;
            Dictionary<string, double> initialGuess = CauchyProblem_CreateInitialGuess();
			InterpolationFunctionType interpolation_type = Enum.Parse<InterpolationFunctionType>(function_type_string);
            foreach (KeyValuePair<string, double> y in initialGuess)
            {
                IEnumerable<Point> calculated_ys = _resultTable[y.Key].Select(pair => new Point(pair.x, pair.yi));
				IInterpolationFunction? interpolation_function = InterpolationBuilder.Build(calculated_ys, interpolation_type);
				if (interpolation_function is null) return;
				double start_x = new Expression(CauchyProblem_InitXTextBox.Text.Trim().Replace(',', '.')).calculate();
				double end_x = new Expression(CauchyProblem_EndXTextBox.Text.Trim().Replace(',', '.')).calculate();
				double step = new Expression(CauchyProblem_StepTextBox.Text.Trim().Replace(',', '.')).calculate() / 5.0;
				int roundNumbers = step.ToString().Contains(',') ? step.ToString().Split(',')[1].Length % 16 : 1;
				var xs = new List<double>();
				var ys = new List<double>();
				for (double x = start_x; Math.Round(x, roundNumbers) <= end_x; x += step)
				{
                    double? current_y = interpolation_function.Calculate(x);
					if (current_y is null) continue;

					xs.Add(x);
					ys.Add((double)current_y);
				}
				double? last_y = interpolation_function.Calculate(end_x);
				if (last_y is not null)
                {
					xs.Add(end_x);
					ys.Add(last_y.Value);
				}
				CauchyProblem_MainChart.Plot.AddScatter(xs.ToArray(), ys.ToArray(), lineWidth: 3, markerSize: 0, label: "i" + y.Key);
			}
			CauchyProblem_MainChart.Refresh();
		}
    }
}
