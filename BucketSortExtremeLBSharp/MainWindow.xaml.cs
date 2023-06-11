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

            InputListBox.ItemsSource = numbers.Select((value, index) => new { Index = index, Value = value });

            var input = ((IEnumerable<dynamic>)InputListBox.ItemsSource).Select(item => (double)item.Value).ToList();

            var descending = DescendingCheckBox.IsChecked ?? false;
            var sortedList = _bucketSort.Sort(input, descending);

            OutputListBox.ItemsSource = sortedList.Select((value, index) => new { Index = index, Value = value });
            ComparisonCountTextBox.Text = _bucketSort.ComparisonCount.ToString();
            SwapCountTextBox.Text = _bucketSort.SwapCount.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PerformanceTestButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var inputSizes = new List<int> { 100, 5000, 10000, 50000, 100000 };
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
                var sortedList = testBucketSort.Sort(numbers, false);
                watch.Stop();

                times.Add(watch.Elapsed.TotalMilliseconds);
            }

            var plotModel = new PlotModel { Title = "Производительность Bucket Sort" };

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
                MarkerSize = 4,
                MarkerStroke = OxyColors.White
            };

            for (int i = 0; i < inputSizes.Count; i++)
            {
                series.Points.Add(new ScatterPoint(inputSizes[i], times[i]));
            }

            plotModel.Series.Add(series);
            plotView.Model = plotModel;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        ATextBox.Clear();
        BTextBox.Clear();
        CTextBox.Clear();
        SizeTextBox.Clear();
        DescendingCheckBox.IsChecked = false;
        InputListBox.ItemsSource = null;
        OutputListBox.ItemsSource = null;
        ComparisonCountTextBox.Clear();
        SwapCountTextBox.Clear();
        plotView.Model = null;
    }
}
