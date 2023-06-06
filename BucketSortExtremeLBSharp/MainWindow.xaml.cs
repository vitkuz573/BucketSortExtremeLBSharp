using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace BucketSortExtremeLBSharp;

public partial class MainWindow : Window
{
    private BucketSort bucketSort;
    private readonly Random random;

    public MainWindow()
    {
        InitializeComponent();
        random = new Random();
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var A = Convert.ToDouble(ATextBox.Text);
            var B = Convert.ToDouble(BTextBox.Text);
            var C = Convert.ToDouble(CTextBox.Text);

            bucketSort = new BucketSort(A, B, C);

            var size = Convert.ToInt32(SizeTextBox.Text);
            var minRange = Convert.ToDouble(MinRangeTextBox.Text);
            var maxRange = Convert.ToDouble(MaxRangeTextBox.Text);

            var numbers = new List<double>();

            for (int i = 0; i < size; i++)
            {
                numbers.Add(Math.Round(minRange + (random.NextDouble() * (maxRange - minRange))));
            }

            InputTextBox.Text = string.Join(",", numbers);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var input = new List<double>(Array.ConvertAll(InputTextBox.Text.Split(','), double.Parse));
            var descending = DescendingCheckBox.IsChecked ?? false;
            var sortedList = bucketSort.Sort(input, descending);

            OutputTextBox.Text = string.Join(",", sortedList);
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
            var inputSizes = new List<int> { 100, 500, 1000, 5000, 10000, 50000, 100000 };
            var times = new List<double>();

            foreach (var size in inputSizes)
            {
                var numbers = new List<double>();

                for (int i = 0; i < size; i++)
                {
                    numbers.Add(Math.Round(random.NextDouble() * 100));
                }

                var watch = Stopwatch.StartNew();
                var sortedList = bucketSort.Sort(numbers, false);
                watch.Stop();

                times.Add(watch.Elapsed.TotalMilliseconds);
            }

            var plotModel = new PlotModel { Title = "Производительность Bucket Sort" };

            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Размер входных данных" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Время (мс)" });

            var series = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White
            };

            for (int i = 0; i < inputSizes.Count; i++)
            {
                series.Points.Add(new DataPoint(inputSizes[i], times[i]));
            }

            plotModel.Series.Add(series);
            plotView.Model = plotModel;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
