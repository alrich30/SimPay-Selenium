using SimPay.SeleniumTests.Infrastructure;
using SimPay.SeleniumTests.Pages;

namespace SimPay.SeleniumTests.Tests;

public sealed class UpdatePaymentTests : SeleniumTestBase
{
    [Test]
    public void UpdatePayment_WithValidData_ShouldUpdatePayment()
    {
        PaymentsPage paymentsPage = Login();

        string originalDescription =
            $"Pago original {Guid.NewGuid():N}";

        string updatedDescription =
            $"Pago actualizado {Guid.NewGuid():N}";

        CreatePayment(
            paymentsPage,
            originalDescription);

        paymentsPage.StartEditingPayment(
            originalDescription);

        paymentsPage.CompleteEditForm(
            "3500.75",
            "USD",
            updatedDescription);

        paymentsPage.Save();

        Assert.That(
            paymentsPage.WaitForMessage(),
            Is.EqualTo("El pago fue actualizado correctamente."));

        Assert.Multiple(() =>
        {
            Assert.That(
                paymentsPage.WaitForPaymentInTable(updatedDescription),
                Is.True);

            Assert.That(
                paymentsPage.WaitForPaymentNotInTable(originalDescription),
                Is.True);
        });
    }

    [Test]
    public void UpdatePayment_WithZeroAmount_ShouldShowValidationError()
    {
        PaymentsPage paymentsPage = Login();

        string originalDescription =
            $"Pago sin actualizar {Guid.NewGuid():N}";

        CreatePayment(
            paymentsPage,
            originalDescription);

        paymentsPage.StartEditingPayment(
            originalDescription);

        paymentsPage.CompleteEditForm(
            "0",
            "DOP",
            "Actualización inválida");

        paymentsPage.Save();

        Assert.Multiple(() =>
        {
            Assert.That(
                paymentsPage.GetAmountValidationMessage(),
                Is.Not.Empty);

            Assert.That(
                paymentsPage.TableContainsPayment(originalDescription),
                Is.True);
        });
    }

    [Test]
    public void UpdatePayment_WithMaximumDescription_ShouldUpdatePayment()
    {
        PaymentsPage paymentsPage = Login();

        string originalDescription =
            $"Pago límite {Guid.NewGuid():N}";

        string boundaryDescription =
            $"Descripción límite {Guid.NewGuid():N}"
                .PadRight(200, 'X');

        CreatePayment(
            paymentsPage,
            originalDescription);

        paymentsPage.StartEditingPayment(
            originalDescription);

        paymentsPage.CompleteEditForm(
            "500.00",
            "DOP",
            boundaryDescription);

        paymentsPage.Save();

        Assert.That(
            paymentsPage.WaitForMessage(),
            Is.EqualTo("El pago fue actualizado correctamente."));

        Assert.That(
            paymentsPage.WaitForPaymentInTable(boundaryDescription),
            Is.True);
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
        string description)
    {
        paymentsPage.CompleteForm(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "1000.00",
            "DOP",
            description);

        paymentsPage.Save();

        Assert.That(
            paymentsPage.WaitForMessage(),
            Is.EqualTo("El pago fue registrado correctamente."));

        Assert.That(
            paymentsPage.WaitForPaymentInTable(description),
            Is.True);
    }
}