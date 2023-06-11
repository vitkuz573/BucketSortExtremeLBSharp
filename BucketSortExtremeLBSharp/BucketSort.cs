using System;
using System.Collections.Generic;

namespace BucketSortExtremeLBSharp;

public class BucketSort
{
    public double A { get; set; }

    public double B { get; set; }
    
    public double C { get; set; }

    public int ComparisonCount { get; private set; }

    public int SwapCount { get; private set; }

    public BucketSort(double A, double B, double C)
    {
        this.A = A;
        this.B = B;
        this.C = C;
    }

    public double F(double x)
    {
        return Math.Exp(-Math.Pow((x - A) / B, -C));
    }

    public double FInverse(double u)
    {
        return A + B * Math.Pow(-Math.Log(u), -1 / C);
    }

    public List<double> Sort(List<double> input, bool descending)
    {
        ComparisonCount = 0;
        SwapCount = 0;

        var n = input.Count;

        var buckets = new List<List<double>>(n + 1);

        for (int i = 0; i <= n; i++)
        {
            buckets.Add(new List<double>());
        }

        for (int i = 0; i < n; i++)
        {
            var bucketIndex = (int)(F(input[i]) * n);

            if (bucketIndex >= n)
            {
                bucketIndex = n;
            }

            buckets[bucketIndex].Add(input[i]);
        }

        for (int i = 0; i <= n; i++)
        {
            if (buckets[i] != null && buckets[i].Count > 1)
            {
                buckets[i] = CustomSort(buckets[i], descending, out int localComparisonCount, out int localSwapCount);
                ComparisonCount += localComparisonCount;
                SwapCount += localSwapCount;
            }
        }

        var sortedList = new List<double>();

        if (descending)
        {
            for (int i = n; i >= 0; i--)
            {
                if (buckets[i] != null)
                {
                    sortedList.AddRange(buckets[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i <= n; i++)
            {
                if (buckets[i] != null)
                {
                    sortedList.AddRange(buckets[i]);
                }
            }
        }

        return sortedList;
    }

    public static List<double> CustomSort(List<double> list, bool descending, out int comparisonCount, out int swapCount)
    {
        comparisonCount = 0;
        swapCount = 0;

        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                comparisonCount++;

                if ((descending && list[i] < list[j]) || (!descending && list[i] > list[j]))
                {
                    swapCount++;

                    (list[j], list[i]) = (list[i], list[j]);
                }
            }
        }

        return list;
    }
}
