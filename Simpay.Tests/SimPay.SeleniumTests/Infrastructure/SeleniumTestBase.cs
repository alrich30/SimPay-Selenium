using AventStack.ExtentReports;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SimPay.SeleniumTests.Infrastructure;

[NonParallelizable]
public abstract class SeleniumTestBase
{
    protected const string BaseUrl = "https://localhost:7058";

    protected IWebDriver Driver { get; private set; } = null!;
    protected WebDriverWait Wait { get; private set; } = null!;

    private ExtentTest _extentTest = null!;

    [SetUp]
    public void SetUp()
    {
        _extentTest = ExtentReportManager.CreateTest(
            TestContext.CurrentContext.Test.Name);

        var options = new ChromeOptions
        {
            AcceptInsecureCertificates = true
        };

        Driver = new ChromeDriver(options);
        Driver.Manage().Window.Maximize();

        Wait = new WebDriverWait(
            Driver,
            TimeSpan.FromSeconds(10));
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            string screenshotPath = CaptureScreenshot();

            string relativeScreenshotPath = Path.Combine(
                "Screenshots",
                Path.GetFileName(screenshotPath))
                .Replace('\\', '/');

            _extentTest.AddScreenCaptureFromPath(
                relativeScreenshotPath,
                "Captura del escenario");

            TestContext.AddTestAttachment(
                screenshotPath,
                "Captura automática del escenario");

            TestStatus status =
                TestContext.CurrentContext.Result.Outcome.Status;

            if (status == TestStatus.Passed)
            {
                _extentTest.Pass("Escenario ejecutado correctamente.");
            }
            else if (status == TestStatus.Failed)
            {
                _extentTest.Fail(
                    TestContext.CurrentContext.Result.Message);
            }
            else
            {
                _extentTest.Skip("La prueba no fue completada.");
            }
        }
        finally
        {
            Driver?.Quit();
            Driver?.Dispose();
            ExtentReportManager.Flush();
        }
    }

    private string CaptureScreenshot()
    {
        string testName = TestContext.CurrentContext.Test.Name;

        foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
        {
            testName = testName.Replace(invalidCharacter, '_');
        }

        string screenshotPath = Path.Combine(
            ExtentReportManager.ScreenshotsDirectory,
            $"{testName}.png");

        Screenshot screenshot =
            ((ITakesScreenshot)Driver).GetScreenshot();

        screenshot.SaveAsFile(screenshotPath);

        return screenshotPath;
    }
}