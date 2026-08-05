using SimPay.SeleniumTests.Infrastructure;
using SimPay.SeleniumTests.Pages;

namespace SimPay.SeleniumTests.Tests;

public sealed class DeletePaymentTests : SeleniumTestBase
{
    [Test]
    public void DeletePayment_WhenConfirmed_ShouldRemovePayment()
    {
        PaymentsPage paymentsPage = Login();

        string description =
            $"Pago para eliminar {Guid.NewGuid():N}";

        CreatePayment(paymentsPage, description);

        paymentsPage.ClickDeleteForPayment(description);
        paymentsPage.AcceptDeleteConfirmation();

        Assert.That(
            paymentsPage.WaitForMessage(),
            Is.EqualTo("El pago fue eliminado correctamente."));

        Assert.That(
            paymentsPage.WaitForPaymentNotInTable(description),
            Is.True);
    }

    [Test]
    public void DeletePayment_WhenCancelled_ShouldKeepPayment()
    {
        PaymentsPage paymentsPage = Login();

        string description =
            $"Pago conservado {Guid.NewGuid():N}";

        CreatePayment(paymentsPage, description);

        paymentsPage.ClickDeleteForPayment(description);
        paymentsPage.CancelDeleteConfirmation();

        Assert.That(
            paymentsPage.TableContainsPayment(description),
            Is.True);
    }

    [Test]
    public void DeletePayment_WithMaximumDescription_ShouldRemovePayment()
    {
        PaymentsPage paymentsPage = Login();

        string description =
            $"Eliminar límite {Guid.NewGuid():N}"
                .PadRight(200, 'X');

        CreatePayment(paymentsPage, description);

        paymentsPage.ClickDeleteForPayment(description);
        paymentsPage.AcceptDeleteConfirmation();

        Assert.That(
            paymentsPage.WaitForMessage(),
            Is.EqualTo("El pago fue eliminado correctamente."));

        Assert.That(
            paymentsPage.WaitForPaymentNotInTable(description),
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
            "750.00",
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