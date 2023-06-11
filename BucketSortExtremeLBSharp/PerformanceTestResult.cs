public class PerformanceTestResult
{
    public int TestNumber { get; set; }

    public double CoefficientA { get; set; }

    public double CoefficientB { get; set; }

    public double CoefficientC { get; set; }

    public int ArraySize { get; set; }

    public string SortDirection { get; set; }

    public double Time { get; set; }

    public double Intercept { get; set; }

    public double Slope { get; set; }

    public double Forecast { get; set; }  // New property for y_hat

    public double Residuals { get; set; } // New property for residuals
}

public class RegressionAnalysisResult
{
    public int Number { get; set; }
    public double Time { get; set; }
    public double ArraySize { get; set; }
    public double ArraySizeSquared { get; set; }
    public double TimeTimesArraySize { get; set; }
    public double Intercept { get; set; }
    public double Slope { get; set; }
    public double Forecast { get; set; }
    public double Residuals { get; set; }
    public double CorrelationCoefficient { get; set; }
    public double DeterminationCoefficient { get; set; }
    public double ElasticityCoefficient { get; set; } // beta coefficient
    public double CoefficientA { get; set; }  // additional property
    public double CoefficientB { get; set; }  // additional property
    public double CoefficientC { get; set; }  // additional property
    public string SortDirection { get; set; }  // additional property
    public int Comparisons { get; set; }  // additional property
    public int Swaps { get; set; }  // additional property
}
