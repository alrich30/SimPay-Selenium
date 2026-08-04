using System.Collections.Concurrent;
using SimPay.Application.Payments;
using SimPay.Domain.Payments;

namespace SimPay.Infrastructure.Payments;

public sealed class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _payments = new();

    public Payment Add(Payment payment)
    {
        var wasAdded = _payments.TryAdd(payment.Id, payment);

        if (!wasAdded)
        {
            throw new InvalidOperationException(
                $"A payment with identifier {payment.Id} already exists.");
        }

        return payment;
    }

    public IReadOnlyCollection<Payment> GetAll()
    {
        return _payments.Values
            .OrderByDescending(payment => payment.CreatedAtUtc)
            .ToArray();
    }

    public Payment? GetById(Guid id)
    {
        _payments.TryGetValue(id, out var payment);

        return payment;
    }

    public Payment? Update(
    Guid id,
    decimal amount,
    string currency,
    string? description)
    {
        var payment = GetById(id);

        if (payment is null)
        {
            return null;
        }

        payment.UpdateDetails(amount, currency, description);

        return payment;
    }

    public bool Delete(Guid id)
    {
        return _payments.TryRemove(id, out _);
    }
}