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

    public double XSquare { get; set; }

    public double XY { get; set; }

    public double Intercept { get; set; }

    public double Slope { get; set; }
}
