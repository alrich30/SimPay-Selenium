using SimPay.Domain.Payments;

namespace Simpay.Tests.Payments;

public sealed class PaymentTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePendingPayment()
    {
        var sourceAccountId = Guid.NewGuid();
        var destinationAccountId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        var payment = new Payment(
            sourceAccountId,
            destinationAccountId,
            1500m,
            "DOP",
            "Pago simulado de prueba");

        var afterCreation = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, payment.Id);
        Assert.Equal(sourceAccountId, payment.SourceAccountId);
        Assert.Equal(destinationAccountId, payment.DestinationAccountId);
        Assert.Equal(1500m, payment.Amount);
        Assert.Equal("DOP", payment.Currency);
        Assert.Equal("Pago simulado de prueba", payment.Description);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.InRange(payment.CreatedAtUtc, beforeCreation, afterCreation);
    }

    [Fact]
    public void Create_WithSameAccountIds_ShouldThrowArgumentException()
    {
        var accountId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            new Payment(
                accountId,
                accountId,
                1500m,
                "DOP",
                "Pago inválido"));
    }

    [Fact]
    public void Create_WithNonPositiveAmount_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Payment(
                Guid.NewGuid(),
                Guid.NewGuid(),
                -500m,
                "DOP",
                "Importe inválido"));
    }

    [Fact]
    public void Create_WithInvalidCurrency_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Payment(
                Guid.NewGuid(),
                Guid.NewGuid(),
                1500m,
                "DO",
                "Moneda inválida"));
    }

    [Fact]
    public void Create_WithLowercaseCurrency_ShouldNormalizeCurrency()
    {
        var payment = new Payment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1500m,
            "dop",
            "Pago válido");

        Assert.Equal("DOP", payment.Currency);
    }
}