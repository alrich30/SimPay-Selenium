using SimPay.Domain.Payments;

namespace SimPay.Application.Payments;

public sealed record PaymentQuery(
    PaymentStatus? Status,
    string? Currency,
    decimal? MinAmount,
    decimal? MaxAmount,
    DateTime? FromDate,
    DateTime? ToDate);