using System;
using System.Collections.Generic;
using System.Windows;

namespace BucketSortExtremeLBSharp;

public partial class MainWindow : Window
{
    private BucketSort bucketSort;
    private Random random;

    public MainWindow()
    {
        InitializeComponent();
        random = new Random();
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double A = Convert.ToDouble(ATextBox.Text);
            double B = Convert.ToDouble(BTextBox.Text);
            double C = Convert.ToDouble(CTextBox.Text);

            bucketSort = new BucketSort(A, B, C);

            int size = Convert.ToInt32(SizeTextBox.Text);
            double minRange = Convert.ToDouble(MinRangeTextBox.Text);
            double maxRange = Convert.ToDouble(MaxRangeTextBox.Text);

            List<double> numbers = new List<double>();
            for (int i = 0; i < size; i++)
            {
                numbers.Add(Math.Round(minRange + (random.NextDouble() * (maxRange - minRange))));
            }

            InputTextBox.Text = string.Join(",", numbers);
        }
        catch (Exception ex)
        {
            OutputTextBox.Text = $"Error: {ex.Message}";
        }
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            List<double> input = new List<double>(Array.ConvertAll(InputTextBox.Text.Split(','), double.Parse));
            bool descending = DescendingCheckBox.IsChecked ?? false;
            List<double> sortedList = bucketSort.Sort(input, descending);
            OutputTextBox.Text = string.Join(",", sortedList);
        }
        catch (Exception ex)
        {
            OutputTextBox.Text = $"Error: {ex.Message}";
        }
    }
}
