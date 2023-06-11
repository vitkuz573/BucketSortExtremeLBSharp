using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace BucketSortExtremeLBSharp;

public partial class MainWindow : Window
{
    private BucketSort _bucketSort;
    private readonly Random _random;

    public MainWindow()
    {
        InitializeComponent();

        _random = new Random();
    }

    private void GenerateAndSortButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var A = Convert.ToDouble(ATextBox.Text);
            var B = Convert.ToDouble(BTextBox.Text);
            var C = Convert.ToDouble(CTextBox.Text);

            _bucketSort = new BucketSort(A, B, C);

            var size = Convert.ToInt32(SizeTextBox.Text);

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
        // 1. Performance Test
        RunPerformanceTest();

        // 2. Regression Analysis
        PerformRegressionAnalysis();
    }

    private void RunPerformanceTest()
    {
        try
        {
            try
            {
                int testCount = Convert.ToInt32(TestCountTextBox.Text);
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

                    for (int i = 0; i < size; i++)
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

                for (int i = 0; i < dataPoints.Count; i++)
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

            var regressionResults = new List<PerformanceTestResult>();

            foreach (var result in testResults)
            {
                var x = result.ArraySize;
                var y = result.Time;
                double xSquare = x * x;
                var xy = x * y;

                var regressionResult = new PerformanceTestResult
                {
                    TestNumber = result.TestNumber,
                    CoefficientA = result.CoefficientA,
                    CoefficientB = result.CoefficientB,
                    CoefficientC = result.CoefficientC,
                    Time = y,
                    ArraySize = x,
                    SortDirection = result.SortDirection,
                    XSquare = xSquare,
                    XY = xy,
                    // These are placeholders; you will need to replace these with the actual regression results
                    Intercept = 0.0,
                    Slope = 0.0,
                };

                regressionResults.Add(regressionResult);
            }

            PerformanceTestListView.ItemsSource = regressionResults;

            // TODO: Add code to compute the actual regression using a library such as Math.NET Numerics
            // For example: Tuple<double, double> p = MathNet.Numerics.Fit.Line(arraySizes.ToArray(), times.ToArray());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
