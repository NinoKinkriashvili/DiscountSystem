using DiscountsSystem.Application.DTOs.Purchases;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Purchases;

public sealed class CreateDirectPurchaseRequestValidator : AbstractValidator<CreateDirectPurchaseRequest>
{
    public CreateDirectPurchaseRequestValidator()
    {
        RuleFor(x => x.OfferId)
            .GreaterThan(0)
            .WithMessage("OfferId must be greater than 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity must be between 1 and 100.");
    }
}