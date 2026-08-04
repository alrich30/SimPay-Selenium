using SimPay.Domain.Payments;
using SimPay.Infrastructure.Payments;

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

    private static Payment CreatePayment(decimal amount, string description)
    {
        return new Payment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            amount,
            "DOP",
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