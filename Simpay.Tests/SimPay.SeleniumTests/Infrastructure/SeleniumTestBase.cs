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
    protected string DownloadDirectory { get; private set; } = null!;

    private ExtentTest _extentTest = null!;

    [SetUp]
    public void SetUp()
    {
        _extentTest = ExtentReportManager.CreateTest(
            TestContext.CurrentContext.Test.Name);

        DownloadDirectory = Path.Combine(
        Path.GetTempPath(),
        "SimPaySeleniumDownloads",
        Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(DownloadDirectory);

        var options = new ChromeOptions
        {
            AcceptInsecureCertificates = true
        };

        options.AddUserProfilePreference(
        "download.default_directory",
        DownloadDirectory);

        options.AddUserProfilePreference(
            "download.prompt_for_download",
            false);

        options.AddUserProfilePreference(
            "download.directory_upgrade",
            true);

        options.AddUserProfilePreference(
            "safebrowsing.enabled",
            true);

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

            try
            {
                if (Directory.Exists(DownloadDirectory))
                {
                    Directory.Delete(
                        DownloadDirectory,
                        recursive: true);
                }
            }
            catch (IOException)
            {
                // Chrome puede tardar brevemente en liberar la descarga.
            }
            catch (UnauthorizedAccessException)
            {
                // La carpeta puede continuar bloqueada temporalmente.
            }
        }
    }

    protected string WaitForDownloadedCsv()
    {
        return Wait.Until(_ =>
        {
            string? csvFile = Directory
                .GetFiles(DownloadDirectory, "*.csv")
                .FirstOrDefault();

            bool downloadInProgress = Directory
                .GetFiles(DownloadDirectory, "*.crdownload")
                .Any();

            if (csvFile is null ||
                downloadInProgress ||
                new FileInfo(csvFile).Length == 0)
            {
                return null;
            }

            return csvFile;
        })!;
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