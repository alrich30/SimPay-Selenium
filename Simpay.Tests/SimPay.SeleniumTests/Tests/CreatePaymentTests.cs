using SimPay.SeleniumTests.Infrastructure;
using SimPay.SeleniumTests.Pages;

namespace SimPay.SeleniumTests.Tests;

public sealed class CreatePaymentTests : SeleniumTestBase
{
    [Test]
    public void CreatePayment_WithValidData_ShouldRegisterPayment()
    {
        PaymentsPage paymentsPage = Login();

        string description =
            $"Pago válido {Guid.NewGuid():N}";

        paymentsPage.CompleteForm(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "1500.50",
            "DOP",
            description);

        paymentsPage.Save();

        string message = paymentsPage.WaitForMessage();

        Assert.Multiple(() =>
        {
            Assert.That(
                message,
                Is.EqualTo("El pago fue registrado correctamente."));

            Assert.That(
                paymentsPage.WaitForPaymentInTable(description),
                Is.True);
        });
    }

    [Test]
    public void CreatePayment_WithSameAccounts_ShouldShowError()
    {
        PaymentsPage paymentsPage = Login();

        string accountId = Guid.NewGuid().ToString();

        string description =
            $"Pago inválido {Guid.NewGuid():N}";

        paymentsPage.CompleteForm(
            accountId,
            accountId,
            "1500.00",
            "DOP",
            description);

        paymentsPage.Save();

        string message = paymentsPage.WaitForMessage();

        Assert.Multiple(() =>
        {
            Assert.That(
                message,
                Is.Not.Empty);

            Assert.That(
                message,
                Is.Not.EqualTo(
                    "El pago fue registrado correctamente."));

            Assert.That(
                paymentsPage.TableContainsPayment(description),
                Is.False);
        });
    }

    [Test]
    public void CreatePayment_WithMaximumAllowedAmount_ShouldRegisterPayment()
    {
        PaymentsPage paymentsPage = Login();

        string description =
            $"Pago límite {Guid.NewGuid():N}";

        paymentsPage.CompleteForm(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "1000000",
            "DOP",
            description);

        paymentsPage.Save();

        string message = paymentsPage.WaitForMessage();

        Assert.Multiple(() =>
        {
            Assert.That(
                message,
                Is.EqualTo("El pago fue registrado correctamente."));

            Assert.That(
                paymentsPage.WaitForPaymentInTable(description),
                Is.True);
        });
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
}