namespace SimPay.Domain.Payments;

public sealed class Payment
{
    public Guid Id { get; private set; }
    public Guid SourceAccountId { get; private set; }
    public Guid DestinationAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public Payment(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        string? description)
    {
        if (sourceAccountId == Guid.Empty)
        {
            throw new ArgumentException(
                "The source account identifier is required.",
                nameof(sourceAccountId));
        }

        if (destinationAccountId == Guid.Empty)
        {
            throw new ArgumentException(
                "The destination account identifier is required.",
                nameof(destinationAccountId));
        }

        if (sourceAccountId == destinationAccountId)
        {
            throw new ArgumentException(
                "Source and destination accounts must be different.",
                nameof(destinationAccountId));
        }

        Id = Guid.NewGuid();
        SourceAccountId = sourceAccountId;
        DestinationAccountId = destinationAccountId;
        Status = PaymentStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;

        UpdateDetails(amount, currency, description);
    }
    public void UpdateDetails(
        decimal amount,
        string currency,
        string? description)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "The payment amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException(
                "The currency is required.",
                nameof(currency));
        }

        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (normalizedCurrency.Length != 3)
        {
            throw new ArgumentException(
                "The currency must contain exactly three characters.",
                nameof(currency));
        }

        if (description?.Length > 200)
        {
            throw new ArgumentException(
                "The description cannot exceed 200 characters.",
                nameof(description));
        }

        Amount = amount;
        Currency = normalizedCurrency;
        Description = description?.Trim();
    }
}