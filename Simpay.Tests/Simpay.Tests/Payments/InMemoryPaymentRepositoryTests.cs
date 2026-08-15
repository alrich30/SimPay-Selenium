using SimPay.Domain.Payments;
using SimPay.Infrastructure.Payments;
using SimPay.Application.Payments;

namespace Simpay.Tests.Payments;

public sealed class InMemoryPaymentRepositoryTests
{
    [Fact]
    public void GetAll_WithStoredPayments_ShouldReturnAllPayments()
    {
        var repository = new InMemoryPaymentRepository();

        var firstPayment = CreatePayment(1000m, "Primer pago");
        var secondPayment = CreatePayment(2000m, "Segundo pago");

        repository.Add(firstPayment);
        repository.Add(secondPayment);

        var payments = repository.GetAll();

        Assert.Equal(2, payments.Count);
        Assert.Contains(firstPayment, payments);
        Assert.Contains(secondPayment, payments);
    }

    [Fact]
    public void GetById_WithExistingId_ShouldReturnPayment()
    {
        var repository = new InMemoryPaymentRepository();
        var payment = CreatePayment(1500m, "Pago existente");

        repository.Add(payment);

        var result = repository.GetById(payment.Id);

        Assert.NotNull(result);
        Assert.Equal(payment.Id, result.Id);
    }

    [Fact]
    public void GetById_WithUnknownId_ShouldReturnNull()
    {
        var repository = new InMemoryPaymentRepository();

        var result = repository.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void Search_WithPendingStatus_ShouldReturnPendingPayments()
    {
        var repository = new InMemoryPaymentRepository();
        var payment = CreatePayment(1000m, "Pago pendiente");

        repository.Add(payment);

        var query = new PaymentQuery(
            PaymentStatus.Pending,
            null,
            null,
            null,
            null,
            null);

        var result = repository.Search(query);

        Assert.Single(result);
        Assert.Contains(payment, result);
    }

    [Fact]
    public void Search_WithCompletedStatus_ShouldReturnEmptyCollection()
    {
        var repository = new InMemoryPaymentRepository();
        repository.Add(CreatePayment(1000m, "Pago pendiente"));

        var query = new PaymentQuery(
            PaymentStatus.Completed,
            null,
            null,
            null,
            null,
            null);

        var result = repository.Search(query);

        Assert.Empty(result);
    }

    [Fact]
    public void Search_WithCurrency_ShouldReturnMatchingPayments()
    {
        var repository = new InMemoryPaymentRepository();
        var dopPayment = CreatePayment(1000m, "Pago en pesos", "DOP");
        var usdPayment = CreatePayment(500m, "Pago en dólares", "USD");

        repository.Add(dopPayment);
        repository.Add(usdPayment);

        var query = new PaymentQuery(
            null,
            "usd",
            null,
            null,
            null,
            null);

        var result = repository.Search(query);

        Assert.Single(result);
        Assert.Contains(usdPayment, result);
    }

    [Fact]
    public void Search_WithAmountRange_ShouldReturnPaymentsInsideRange()
    {
        var repository = new InMemoryPaymentRepository();
        var lowPayment = CreatePayment(100m, "Pago menor");
        var matchingPayment = CreatePayment(1000m, "Pago dentro del rango");
        var highPayment = CreatePayment(5000m, "Pago mayor");

        repository.Add(lowPayment);
        repository.Add(matchingPayment);
        repository.Add(highPayment);

        var query = new PaymentQuery(
            null,
            null,
            500m,
            2000m,
            null,
            null);

        var result = repository.Search(query);

        Assert.Single(result);
        Assert.Contains(matchingPayment, result);
    }

    [Fact]
    public void Search_WithDateRange_ShouldReturnPaymentsCreatedToday()
    {
        var repository = new InMemoryPaymentRepository();
        var payment = CreatePayment(1000m, "Pago de hoy");

        repository.Add(payment);

        var today = DateTime.UtcNow.Date;

        var query = new PaymentQuery(
            null,
            null,
            null,
            null,
            today,
            today);

        var result = repository.Search(query);

        Assert.Single(result);
        Assert.Contains(payment, result);
    }

    [Fact]
    public void Search_WithCombinedFilters_ShouldReturnOnlyMatchingPayment()
    {
        var repository = new InMemoryPaymentRepository();
        var matchingPayment = CreatePayment(1500m, "Pago coincidente", "USD");
        var wrongCurrency = CreatePayment(1500m, "Moneda diferente", "DOP");
        var wrongAmount = CreatePayment(5000m, "Monto diferente", "USD");

        repository.Add(matchingPayment);
        repository.Add(wrongCurrency);
        repository.Add(wrongAmount);

        var query = new PaymentQuery(
            PaymentStatus.Pending,
            "USD",
            1000m,
            2000m,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date);

        var result = repository.Search(query);

        Assert.Single(result);
        Assert.Contains(matchingPayment, result);
    }

    private static Payment CreatePayment(
        decimal amount,
        string description,
        string currency = "DOP")
    {
        return new Payment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            amount,
            currency,
            description);
    }

    [Fact]
    public void Update_WithExistingId_ShouldModifyPayment()
    {
        var repository = new InMemoryPaymentRepository();
        var payment = CreatePayment(1500m, "Descripción original");

        repository.Add(payment);

        var updatedPayment = repository.Update(
            payment.Id,
            3250.75m,
            "DOP",
            "Pago actualizado");

        Assert.NotNull(updatedPayment);
        Assert.Equal(payment.Id, updatedPayment.Id);
        Assert.Equal(3250.75m, updatedPayment.Amount);
        Assert.Equal("DOP", updatedPayment.Currency);
        Assert.Equal("Pago actualizado", updatedPayment.Description);
    }

    [Fact]
    public void Update_WithUnknownId_ShouldReturnNull()
    {
        var repository = new InMemoryPaymentRepository();

        var result = repository.Update(
            Guid.NewGuid(),
            3250.75m,
            "DOP",
            "Pago inexistente");

        Assert.Null(result);
    }

    [Fact]
    public void Delete_WithExistingId_ShouldRemovePayment()
    {
        var repository = new InMemoryPaymentRepository();
        var payment = CreatePayment(1500m, "Pago para eliminar");

        repository.Add(payment);

        var wasDeleted = repository.Delete(payment.Id);
        var deletedPayment = repository.GetById(payment.Id);

        Assert.True(wasDeleted);
        Assert.Null(deletedPayment);
    }

    [Fact]
    public void Delete_WithUnknownId_ShouldReturnFalse()
    {
        var repository = new InMemoryPaymentRepository();

        var wasDeleted = repository.Delete(Guid.NewGuid());

        Assert.False(wasDeleted);
    }
}