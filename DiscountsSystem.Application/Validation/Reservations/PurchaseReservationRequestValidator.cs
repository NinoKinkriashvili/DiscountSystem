using DiscountsSystem.Application.DTOs.Reservations;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Reservations;

public sealed class PurchaseReservationRequestValidator : AbstractValidator<PurchaseReservationRequest>
{
    public PurchaseReservationRequestValidator()
    {
        RuleFor(x => x.ReservationId)
            .GreaterThan(0)
            .WithMessage("ReservationId must be greater than 0.");
    }
}