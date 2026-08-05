using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SimPay.SeleniumTests.Pages;

public sealed class PaymentsPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private readonly string _baseUrl;

    public PaymentsPage(
        IWebDriver driver,
        WebDriverWait wait,
        string baseUrl)
    {
        _driver = driver;
        _wait = wait;
        _baseUrl = baseUrl;
    }

    private IWebElement SourceAccountInput =>
        _driver.FindElement(By.Id("source-account-id"));

    private IWebElement DestinationAccountInput =>
        _driver.FindElement(By.Id("destination-account-id"));

    private IWebElement AmountInput =>
        _driver.FindElement(By.Id("amount"));

    private IWebElement CurrencyInput =>
        _driver.FindElement(By.Id("currency"));

    private IWebElement DescriptionInput =>
        _driver.FindElement(By.Id("description"));

    private IWebElement SaveButton =>
        _driver.FindElement(By.Id("save-button"));

    private IWebElement PaymentMessage =>
        _driver.FindElement(By.Id("payment-message"));

    private IWebElement PaymentsBody =>
        _driver.FindElement(By.Id("payments-body"));

    private IWebElement RefreshButton =>
    _driver.FindElement(By.Id("refresh-button"));

    public void Open()
    {
        _driver.Navigate().GoToUrl(
            $"{_baseUrl}/payments.html");
    }

    public void CompleteForm(
        string sourceAccountId,
        string destinationAccountId,
        string amount,
        string currency,
        string description)
    {
        SourceAccountInput.Clear();
        SourceAccountInput.SendKeys(sourceAccountId);

        DestinationAccountInput.Clear();
        DestinationAccountInput.SendKeys(destinationAccountId);

        AmountInput.Clear();
        AmountInput.SendKeys(amount);

        CurrencyInput.Clear();
        CurrencyInput.SendKeys(currency);

        DescriptionInput.Clear();
        DescriptionInput.SendKeys(description);
    }

    public void Save()
    {
        SaveButton.Click();
    }

    public string WaitForMessage()
    {
        return _wait.Until(driver =>
        {
            IWebElement message =
                driver.FindElement(By.Id("payment-message"));

            return string.IsNullOrWhiteSpace(message.Text)
                ? null
                : message.Text;
        })!;
    }

    public bool WaitForPaymentInTable(string description)
    {
        return _wait.Until(driver =>
        {
            string tableContent =
                driver.FindElement(By.Id("payments-body")).Text;

            return tableContent.Contains(
                description,
                StringComparison.OrdinalIgnoreCase);
        });
    }

    public string GetAmountValidationMessage()
    {
        return AmountInput.GetAttribute("validationMessage") ?? "";
    }

    public bool TableContainsPayment(string description)
    {
        return PaymentsBody.Text.Contains(
            description,
            StringComparison.OrdinalIgnoreCase);
    }


    public void RefreshPayments()
    {
        RefreshButton.Click();
    }

    public void WaitForLoginRedirect()
    {
        _wait.Until(driver =>
            driver.Url.Contains(
                "index.html",
                StringComparison.OrdinalIgnoreCase));
    }

    public void StartEditingPayment(string description)
    {
        IWebElement row = WaitForPaymentRow(description);

        row.FindElement(By.CssSelector(".edit-button"))
            .Click();

        _wait.Until(driver =>
            driver.FindElement(By.Id("form-title")).Text ==
            "Actualizar pago");
    }

    public void CompleteEditForm(
    string amount,
    string currency,
    string description)
    {
        AmountInput.Clear();
        AmountInput.SendKeys(amount);

        CurrencyInput.Clear();
        CurrencyInput.SendKeys(currency);

        DescriptionInput.Clear();
        DescriptionInput.SendKeys(description);
    }

    public bool WaitForPaymentNotInTable(string description)
    {
        return _wait.Until(driver =>
        {
            string tableContent =
                driver.FindElement(By.Id("payments-body")).Text;

            return !tableContent.Contains(
                description,
                StringComparison.OrdinalIgnoreCase);
        });
    }

    private IWebElement WaitForPaymentRow(string description)
    {
        return _wait.Until(driver =>
        {
            return driver
                .FindElements(By.CssSelector("#payments-body tr"))
                .FirstOrDefault(row =>
                    row.Text.Contains(
                        description,
                        StringComparison.OrdinalIgnoreCase));
        })!;
    }
}