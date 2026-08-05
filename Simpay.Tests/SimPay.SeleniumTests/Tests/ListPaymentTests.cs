using SimPay.SeleniumTests.Infrastructure;
using SimPay.SeleniumTests.Pages;

namespace SimPay.SeleniumTests.Tests;

public sealed class ListPaymentTests : SeleniumTestBase
{
    [Test]
    public void ListPayments_WithRegisteredPayment_ShouldDisplayPayment()
    {
        PaymentsPage paymentsPage = Login();

        string description =
            $"Pago para consulta {Guid.NewGuid():N}";

        CreatePayment(
            paymentsPage,
            description,
            "2750.50");

        paymentsPage.RefreshPayments();

        Assert.That(
            paymentsPage.WaitForPaymentInTable(description),
            Is.True);
    }

    [Test]
    public void ListPayments_WithoutLogin_ShouldRedirectToLoginPage()
    {
        PaymentsPage paymentsPage =
            new(Driver, Wait, BaseUrl);

        paymentsPage.Open();
        paymentsPage.WaitForLoginRedirect();

        Assert.That(
            Driver.Url,
            Does.Contain("index.html"));
    }

    [Test]
    public void ListPayments_WithSeveralPayments_ShouldDisplayAllPayments()
    {
        PaymentsPage paymentsPage = Login();

        string[] descriptions =
        {
            $"Pago múltiple 1 {Guid.NewGuid():N}",
            $"Pago múltiple 2 {Guid.NewGuid():N}",
            $"Pago múltiple 3 {Guid.NewGuid():N}",
            $"Pago múltiple 4 {Guid.NewGuid():N}",
            $"Pago múltiple 5 {Guid.NewGuid():N}"
        };

        foreach (string description in descriptions)
        {
            CreatePayment(
                paymentsPage,
                description,
                "100.00");
        }

        paymentsPage.RefreshPayments();

        foreach (string description in descriptions)
        {
            Assert.That(
                paymentsPage.WaitForPaymentInTable(description),
                Is.True,
                $"No se encontró el pago: {description}");
        }
    }

    private PaymentsPage Login()
    {
        LoginPage loginPage =
            new(Driver, Wait, BaseUrl);

        loginPage.Open();
        loginPage.Login("admin", "SimPay123!");
        loginPage.WaitForPaymentsPage();

        return new PaymentsPage(
            Driver,
            Wait,
            BaseUrl);
    }

    private static void CreatePayment(
        PaymentsPage paymentsPage,
        string description,
        string amount)
    {
        paymentsPage.CompleteForm(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            amount,
            "DOP",
            description);

        paymentsPage.Save();

        Assert.That(
            paymentsPage.WaitForMessage(),
            Is.EqualTo("El pago fue registrado correctamente."));
    }
}