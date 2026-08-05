using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace SimPay.SeleniumTests.Infrastructure;

public static class ExtentReportManager
{
    private static readonly string ProjectDirectory =
        Path.GetFullPath(Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "..", "..", ".."));

    public static readonly string ArtifactsDirectory =
        Path.Combine(ProjectDirectory, "TestArtifacts");

    public static readonly string ScreenshotsDirectory =
        Path.Combine(ArtifactsDirectory, "Screenshots");

    private static readonly ExtentReports Report = CreateReport();

    private static ExtentReports CreateReport()
    {
        Directory.CreateDirectory(ArtifactsDirectory);
        Directory.CreateDirectory(ScreenshotsDirectory);

        string reportPath =
            Path.Combine(ArtifactsDirectory, "SeleniumReport.html");

        var reporter = new ExtentSparkReporter(reportPath);

        reporter.Config.DocumentTitle = "Reporte de pruebas de SimPay";
        reporter.Config.ReportName = "Pruebas automatizadas con Selenium";

        var report = new ExtentReports();
        report.AttachReporter(reporter);

        report.AddSystemInfo("Aplicación", "SimPay");
        report.AddSystemInfo("Navegador", "Google Chrome");
        report.AddSystemInfo("Framework", "Selenium con NUnit");

        return report;
    }

    public static ExtentTest CreateTest(string testName)
    {
        return Report.CreateTest(testName);
    }

    public static void Flush()
    {
        Report.Flush();
    }
}