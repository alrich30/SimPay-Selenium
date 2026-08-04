using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SimPay.SeleniumTests.Pages;

public sealed class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private readonly string _baseUrl;

    private readonly By _usernameInput = By.Id("username");
    private readonly By _passwordInput = By.Id("password");
    private readonly By _loginButton = By.Id("login-button");
    private readonly By _loginMessage = By.Id("login-message");

    public LoginPage(
        IWebDriver driver,
        WebDriverWait wait,
        string baseUrl)
    {
        _driver = driver;
        _wait = wait;
        _baseUrl = baseUrl;
    }

    public void Open()
    {
        _driver.Navigate().GoToUrl($"{_baseUrl}/index.html");
    }

    public void Login(string username, string password)
    {
        IWebElement usernameElement =
            _driver.FindElement(_usernameInput);

        IWebElement passwordElement =
            _driver.FindElement(_passwordInput);

        usernameElement.Clear();
        usernameElement.SendKeys(username);

        passwordElement.Clear();
        passwordElement.SendKeys(password);

        _driver.FindElement(_loginButton).Click();
    }

    public void SubmitEmptyForm()
    {
        _driver.FindElement(_loginButton).Click();
    }

    public void WaitForPaymentsPage()
    {
        _wait.Until(driver =>
            driver.Url.Contains("payments.html"));
    }

    public string WaitForErrorMessage()
    {
        return _wait.Until(driver =>
        {
            string message =
                driver.FindElement(_loginMessage).Text;

            return string.IsNullOrWhiteSpace(message)
                ? null
                : message;
        })!;
    }

    public string GetUsernameValidationMessage()
    {
        return _driver
            .FindElement(_usernameInput)
            .GetAttribute("validationMessage") ?? "";
    }

    public bool IsLoginPage()
    {
        return _driver.Url.Contains("index.html");
    }
}