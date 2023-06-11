namespace BucketSortExtremeLBSharp;

public class PerformanceTestResult
{
    public int TestNumber { get; set; }

    public double CoefficientA { get; set; }

    public double CoefficientB { get; set; }

    public double CoefficientC { get; set; }

    public int ArraySize { get; set; }

    public string SortDirection { get; set; }

    public double Time { get; set; }

    public double ArraySizeSquared { get; set; }

    public double TimeTimesArraySize { get; set; }

    public double ElasticityCoefficient { get; set; }
}
