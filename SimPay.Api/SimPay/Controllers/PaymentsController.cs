using Microsoft.AspNetCore.Mvc;
using SimPay.Application.Payments;
using SimPay.Domain.Payments;

namespace SimPay.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentsController(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Payment>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IReadOnlyCollection<Payment>> GetAll(
    [FromQuery] PaymentStatus? status,
    [FromQuery] string? currency,
    [FromQuery] decimal? minAmount,
    [FromQuery] decimal? maxAmount,
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate)
    {
        if (minAmount.HasValue && minAmount.Value < 0)
        {
            return BadRequest(new
            {
                message = "El monto mínimo no puede ser negativo."
            });
        }

        if (maxAmount.HasValue && maxAmount.Value < 0)
        {
            return BadRequest(new
            {
                message = "El monto máximo no puede ser negativo."
            });
        }

        if (minAmount.HasValue &&
            maxAmount.HasValue &&
            minAmount.Value > maxAmount.Value)
        {
            return BadRequest(new
            {
                message = "El monto mínimo no puede superar el monto máximo."
            });
        }

        if (fromDate.HasValue &&
            toDate.HasValue &&
            fromDate.Value.Date > toDate.Value.Date)
        {
            return BadRequest(new
            {
                message = "La fecha inicial no puede ser posterior a la fecha final."
            });
        }

        if (!string.IsNullOrWhiteSpace(currency) &&
            currency.Trim().Length != 3)
        {
            return BadRequest(new
            {
                message = "La moneda debe contener exactamente tres caracteres."
            });
        }

        var query = new PaymentQuery(
            status,
            currency,
            minAmount,
            maxAmount,
            fromDate,
            toDate);

        var payments = _paymentRepository.Search(query);

        return Ok(payments);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Payment> GetById(Guid id)
    {
        var payment = _paymentRepository.GetById(id);

        if (payment is null)
        {
            return NotFound();
        }

        return Ok(payment);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Payment> Update(
    Guid id,
    UpdatePaymentRequest request)
    {
        var payment = _paymentRepository.Update(
            id,
            request.Amount,
            request.Currency.Trim().ToUpperInvariant(),
            request.Description);

        if (payment is null)
        {
            return NotFound();
        }

        return Ok(payment);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var wasDeleted = _paymentRepository.Delete(id);

        if (!wasDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(Payment), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Payment> Create(CreatePaymentRequest request)
    {
        if (request.SourceAccountId == Guid.Empty)
        {
            return BadRequest(new
            {
                message = "La cuenta de origen es obligatoria."
            });
        }

        if (request.DestinationAccountId == Guid.Empty)
        {
            return BadRequest(new
            {
                message = "La cuenta de destino es obligatoria."
            });
        }

        if (request.SourceAccountId == request.DestinationAccountId)
        {
            return BadRequest(new
            {
                message = "Las cuentas de origen y destino deben ser diferentes."
            });
        }

        var payment = new Payment(
            request.SourceAccountId,
            request.DestinationAccountId,
            request.Amount,
            request.Currency.Trim().ToUpperInvariant(),
            request.Description);

        _paymentRepository.Add(payment);

        return CreatedAtAction(
            nameof(GetById),
            new { id = payment.Id },
            payment);
        }
}