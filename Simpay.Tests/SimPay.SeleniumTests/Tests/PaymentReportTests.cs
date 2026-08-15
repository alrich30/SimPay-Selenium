using SimPay.SeleniumTests.Infrastructure;
using SimPay.SeleniumTests.Pages;

namespace SimPay.SeleniumTests.Tests;

public sealed class PaymentReportTests : SeleniumTestBase
{
    [Test]
    public void FilterPayments_WithCurrency_ShouldDisplayOnlyMatchingPayments()
    {
        PaymentsPage paymentsPage = Login();

        string filteredCurrency = CreateUniqueCurrency();
        string otherCurrency = CreateUniqueCurrency();

        string matchingDescription =
            $"Pago filtrado {Guid.NewGuid():N}";

        string excludedDescription =
            $"Pago excluido {Guid.NewGuid():N}";

        CreatePayment(
            paymentsPage,
            matchingDescription,
            "750.00",
            filteredCurrency);

        CreatePayment(
            paymentsPage,
            excludedDescription,
            "900.00",
            otherCurrency);

        paymentsPage.ApplyCurrencyFilter(filteredCurrency);

        Assert.That(
            paymentsPage.WaitForPaymentInTable(matchingDescription),
            Is.True);

        Assert.That(
            paymentsPage.WaitForPaymentNotInTable(excludedDescription),
            Is.True);
    }

    [Test]
    public void Statistics_WithCurrencyFilter_ShouldShowFilteredTotals()
    {
        PaymentsPage paymentsPage = Login();

        string filteredCurrency = CreateUniqueCurrency();
        string otherCurrency = CreateUniqueCurrency();

        CreatePayment(
            paymentsPage,
            $"Estadística 1 {Guid.NewGuid():N}",
            "100.00",
            filteredCurrency);

        CreatePayment(
            paymentsPage,
            $"Estadística 2 {Guid.NewGuid():N}",
            "200.00",
            filteredCurrency);

        CreatePayment(
            paymentsPage,
            $"Estadística excluida {Guid.NewGuid():N}",
            "300.00",
            otherCurrency);

        paymentsPage.ApplyCurrencyFilter(filteredCurrency);

        Assert.That(
            paymentsPage.WaitForTotalPayments(2),
            Is.EqualTo(2));

        Assert.That(
            paymentsPage.GetPendingPayments(),
            Is.EqualTo(2));
    }

    [Test]
    public void ExportCsv_WithFilteredPayments_ShouldDownloadMatchingPayment()
    {
        PaymentsPage paymentsPage = Login();

        string filteredCurrency = CreateUniqueCurrency();

        string description =
            $"Pago exportado {Guid.NewGuid():N}";

        CreatePayment(
            paymentsPage,
            description,
            "1250.50",
            filteredCurrency);

        paymentsPage.ApplyCurrencyFilter(filteredCurrency);

        Assert.That(
            paymentsPage.WaitForPaymentInTable(description),
            Is.True);

        Assert.That(
            paymentsPage.IsExportEnabled(),
            Is.True);

        paymentsPage.ExportCsv();

        string csvPath = WaitForDownloadedCsv();
        string csvContent = File.ReadAllText(csvPath);

        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(csvPath), Is.True);
            Assert.That(csvContent, Does.Contain(description));
            Assert.That(csvContent, Does.Contain(filteredCurrency));
            Assert.That(csvContent, Does.Contain("1250.5"));
        });

        Assert.That(
            paymentsPage.WaitForFilterMessage(),
            Does.Contain("Se exportaron"));
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
        string amount,
        string currency)
    {
        paymentsPage.CompleteForm(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            amount,
            currency,
            description);

        paymentsPage.Save();

        Assert.That(
            paymentsPage.WaitForMessage(),
            Is.EqualTo(
                "El pago fue registrado correctamente."));
    }

    private static string CreateUniqueCurrency()
    {
        return Guid.NewGuid()
            .ToString("N")[..3]
            .ToUpperInvariant();
    }
}