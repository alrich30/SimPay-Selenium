using System.ComponentModel.DataAnnotations;

namespace SimPay.Application.Payments;

public sealed record UpdatePaymentRequest(
    [Range(
        typeof(decimal),
        "0.01",
        "999999999.99")]
    decimal Amount,

    [Required]
    [StringLength(3, MinimumLength = 3)]
    string Currency,

    [StringLength(200)]
    string? Description);