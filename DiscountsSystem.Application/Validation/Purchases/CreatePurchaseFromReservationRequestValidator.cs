using DiscountsSystem.Application.DTOs.Purchases;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Purchases;

public sealed class CreatePurchaseFromReservationRequestValidator : AbstractValidator<CreatePurchaseFromReservationRequest>
{
    public CreatePurchaseFromReservationRequestValidator()
    {
        RuleFor(x => x.ReservationId)
            .GreaterThan(0)
            .WithMessage("ReservationId must be greater than 0.");
    }
}