using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace BucketSortExtremeLBSharp;

public partial class MainWindow : System.Windows.Window
{
    private BucketSort _bucketSort;
    private readonly Random _random;

    public MainWindow()
    {
        InitializeComponent();

        _random = new Random();
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
        e.Handled = !IsTextAllowed(fullText);
    }

    private static bool IsTextAllowed(string text)
    {
        text = text.Replace(',', '.');  // заменяем запятые на точки

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))  // используем InvariantCulture, ожидающую точку в качестве десятичного разделителя
        {
            return number != 0.0;
        }

        return false;
    }

    private void SizeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
        e.Handled = !IsSizeTextAllowed(fullText);
    }

    private static bool IsSizeTextAllowed(string text)
    {
        if (int.TryParse(text, out var number))
        {
            return number >= 1;
        }

        return false;
    }

    private void TestCountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
        e.Handled = !IsTestCountTextAllowed(fullText);
    }

    private static bool IsTestCountTextAllowed(string text)
    {
        if (int.TryParse(text, out var number))
        {
            return number >= 2;
        }

        return false;
    }

    private void GenerateAndSortButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Проверка, что поля не пустые и значения можно преобразовать в double
            var culture = CultureInfo.InvariantCulture;

            if (string.IsNullOrEmpty(ATextBox.Text) ||
                string.IsNullOrEmpty(BTextBox.Text) ||
                string.IsNullOrEmpty(CTextBox.Text) ||
                !double.TryParse(ATextBox.Text.Replace(',', '.'), NumberStyles.Float, culture, out double A) ||
                !double.TryParse(BTextBox.Text.Replace(',', '.'), NumberStyles.Float, culture, out double B) ||
                !double.TryParse(CTextBox.Text.Replace(',', '.'), NumberStyles.Float, culture, out double C))
            {
                MessageBox.Show("Пожалуйста, введите валидные коэффициенты (A, B, C).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _bucketSort = new BucketSort(A, B, C);

            // Проверка, что поле не пустое и значение можно преобразовать в int
            if (string.IsNullOrEmpty(SizeTextBox.Text) ||
                !int.TryParse(SizeTextBox.Text, out int size))
            {
                MessageBox.Show("Пожалуйста, введите валидный размер массива.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var numbers = new List<double>();

            for (int i = 0; i < size; i++)
            {
                var u = _random.NextDouble();
                numbers.Add(_bucketSort.FInverse(u));
            }

            InputListView.ItemsSource = numbers.Select((value, index) => new { Index = index + 1, Value = value });

            var input = ((IEnumerable<dynamic>)InputListView.ItemsSource).Select(item => (double)item.Value).ToList();

            var descending = DescendingCheckBox.IsChecked ?? false;

            var watch = Stopwatch.StartNew();
            var sortedList = _bucketSort.Sort(input, descending);
            watch.Stop();

            OutputListView.ItemsSource = sortedList.Select((value, index) => new { Index = index + 1, Value = value });
            ComparisonCountTextBox.Text = _bucketSort.ComparisonCount.ToString();
            SwapCountTextBox.Text = _bucketSort.SwapCount.ToString();
            ElapsedTimeTextBox.Text = $"{watch.Elapsed.TotalMilliseconds} ms";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RunPerformanceTestAndRegressionAnalysisButton_Click(object sender, RoutedEventArgs e)
    {
        RunPerformanceTest();
        PerformRegressionAnalysis();
    }

    private void RunPerformanceTest()
    {
        try
        {
            try
            {
                // Проверка, что поле не пустое и значение можно преобразовать в int
                if (string.IsNullOrEmpty(TestCountTextBox.Text) || !int.TryParse(TestCountTextBox.Text, out int testCount))
                {
                    MessageBox.Show("Пожалуйста, введите валидное количество тестов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    return;
                }

                var inputSizes = new List<int>();
                var performanceResults = new List<PerformanceTestResult>();

                for (int i = 0; i < testCount; i++)
                {
                    inputSizes.Add(_random.Next(100, 50001));
                }

                var times = new List<double>();

                var testBucketSort = new BucketSort(1, 1, 1);

                foreach (var size in inputSizes)
                {
                    var numbers = new List<double>();

                    for (var i = 0; i < size; i++)
                    {
                        numbers.Add(Math.Round(_random.NextDouble() * 100));
                    }

                    var watch = Stopwatch.StartNew();
                    var descending = DescendingCheckBox.IsChecked ?? false;
                    var sortedList = testBucketSort.Sort(numbers, false);
                    watch.Stop();

                    times.Add(watch.Elapsed.TotalMilliseconds);

                    performanceResults.Add(new PerformanceTestResult
                    {
                        TestNumber = performanceResults.Count + 1,
                        CoefficientA = testBucketSort.A,
                        CoefficientB = testBucketSort.B,
                        CoefficientC = testBucketSort.C,
                        ArraySize = size,
                        SortDirection = descending ? "Убывание" : "Возрастание",
                        Time = watch.Elapsed.TotalMilliseconds
                    });
                }

                PerformanceTestListView.ItemsSource = performanceResults;

                var plotModel = new PlotModel();

                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Размер входных данных",
                    MajorGridlineStyle = LineStyle.Solid,
                    MajorGridlineColor = OxyColors.LightGray,
                    MajorGridlineThickness = 1
                };

                var yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Время (мс)",
                    MajorGridlineStyle = LineStyle.Solid,
                    MajorGridlineColor = OxyColors.LightGray,
                    MajorGridlineThickness = 1
                };

                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);

                var series = new ScatterSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerStroke = OxyColors.Black,
                    MarkerFill = OxyColors.Red,
                    MarkerSize = 5,
                };

                var dataPoints = inputSizes.Select((inputSize, index) => new { InputSize = inputSize, Time = times[index] })
                                           .OrderBy(dataPoint => dataPoint.InputSize).ToList();

                for (var i = 0; i < dataPoints.Count; i++)
                {
                    series.Points.Add(new ScatterPoint(dataPoints[i].InputSize, dataPoints[i].Time));
                }

                plotModel.Series.Add(series);
                PerformanceTestPlot.Model = plotModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PerformRegressionAnalysis()
    {
        try
        {
            var testResults = (IEnumerable<PerformanceTestResult>)PerformanceTestListView.ItemsSource;

            var arraySizes = testResults.Select(result => result.ArraySize).ToArray();
            var times = testResults.Select(result => result.Time).ToArray();

            var arraySizesDouble = arraySizes.Select(i => (double)i).ToArray();
            var timesDouble = times.Select(i => (double)i).ToArray();

            // Определение коэффициентов линейной регрессии
            var (A, B) = Fit.Line(arraySizesDouble, timesDouble);

            // Коэффициент корреляции Пирсона
            var correlationCoefficient = Correlation.Pearson(arraySizesDouble, timesDouble);

            // Коэффициент детерминации
            var determinationCoefficient = Math.Pow(correlationCoefficient, 2);

            // Построение системы нормальных уравнений
            var sumY = timesDouble.Sum();
            var sumX = arraySizesDouble.Sum();
            var sumXY = timesDouble.Zip(arraySizesDouble, (t, a) => t * a).Sum();
            var sumXSquare = arraySizesDouble.Select(a => a * a).Sum();
            var n = timesDouble.Length;

            // Построение системы нормальных уравнений
            var systemNormalEquations = new FlowDocument();
            systemNormalEquations.Blocks.Add(new Paragraph(new Run($"{sumY} = {n}*a0 + a1*{sumX}\n{sumXY} = a0*{sumX} + a1*{sumXSquare}")));
            SystemNormalEquationsTextBox.Document = systemNormalEquations;

            // Построение уравнения связи
            var equation = new FlowDocument();
            equation.Blocks.Add(new Paragraph(new Run($"y = {A} + {B}*x")));
            EquationTextBox.Document = equation;

            var regressionResults = new List<PerformanceTestResult>();

            foreach (var result in testResults)
            {
                var elasticityCoefficient = B * (result.ArraySize / result.Time);

                regressionResults.Add(new PerformanceTestResult
                {
                    TestNumber = result.TestNumber,
                    Time = result.Time,
                    ArraySize = result.ArraySize,
                    ArraySizeSquared = Math.Pow(result.ArraySize, 2),
                    TimeTimesArraySize = result.Time * result.ArraySize,
                    ElasticityCoefficient = elasticityCoefficient
                });
            }

            CorrelationCoefficientTextBox.Text = correlationCoefficient.ToString();
            DeterminationCoefficientTextBox.Text = determinationCoefficient.ToString();
            A0CoefficientTextBox.Text = A.ToString();
            A1CoefficientTextBox.Text = B.ToString();

            PerformanceTestListView.ItemsSource = regressionResults;

            // Находим минимальное и максимальное значения x
            double minX = arraySizesDouble.Min();
            double maxX = arraySizesDouble.Max();

            var regressionLine = new LineSeries
            {
                StrokeThickness = 2,
                Color = OxyColors.Blue,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
                Title = "Линия регрессии",
            };
            
            // Расчет y (прогнозируемое время) для минимального и максимального значения x и добавление точек в LineSeries
            regressionLine.Points.Add(new DataPoint(minX, A + B * minX));
            regressionLine.Points.Add(new DataPoint(maxX, A + B * maxX));

            // Добавление линии регресии на график
            PerformanceTestPlot.Model.Series.Add(regressionLine);

            PerformanceTestPlot.Model.InvalidatePlot(true); // Обновление графика
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
