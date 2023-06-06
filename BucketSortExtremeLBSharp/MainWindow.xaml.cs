using System;
using System.Collections.Generic;
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
            OutputTextBox.Text = $"Error: {ex.Message}";
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
            OutputTextBox.Text = $"Error: {ex.Message}";
        }
    }
}
