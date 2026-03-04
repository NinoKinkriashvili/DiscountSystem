using DiscountsSystem.Application.DTOs.Reservations;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Reservations;

public sealed class CancelReservationRequestValidator : AbstractValidator<CancelReservationRequest>
{
    public CancelReservationRequestValidator()
    {
        RuleFor(x => x.ReservationId)
            .GreaterThan(0)
            .WithMessage("ReservationId must be greater than 0.");
    }
}