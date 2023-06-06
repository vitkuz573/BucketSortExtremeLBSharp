using System;
using System.Collections.Generic;

namespace BucketSortExtremeLBSharp;

public class BucketSort
{
    private readonly double A;
    private readonly double B;
    private readonly double C;

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
        var n = input.Count;

        var buckets = new List<List<double>>(n);
        for (int i = 0; i < n; i++)
        {
            buckets.Add(new List<double>());
        }

        for (int i = 0; i < n; i++)
        {
            int bucketIndex = (int)(F(input[i]) * n);

            if (bucketIndex == n)
            {
                bucketIndex--;
            }

            buckets[bucketIndex].Add(input[i]);
        }

        for (int i = 0; i < n; i++)
        {
            buckets[i]?.Sort();
            if (descending)
            {
                buckets[i]?.Reverse();
            }
        }

        var sortedList = new List<double>();

        if (descending)
        {
            for (int i = n - 1; i >= 0; i--)
            {
                if (buckets[i] != null)
                {
                    sortedList.AddRange(buckets[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < n; i++)
            {
                if (buckets[i] != null)
                {
                    sortedList.AddRange(buckets[i]);
                }
            }
        }

        return sortedList;
    }
}
