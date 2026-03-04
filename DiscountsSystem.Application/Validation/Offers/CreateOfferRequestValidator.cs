using DiscountsSystem.Application.DTOs.Offers;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Offers;

public sealed class CreateOfferRequestValidator : OfferRequestBaseValidator<CreateOfferRequest>
{
    public CreateOfferRequestValidator()
    {
        ApplyCommonRules(
            x => x.Title,
            x => x.Description,
            x => x.OriginalPrice,
            x => x.DiscountPrice,
            x => x.CouponQuantityTotal,
            x => x.StartDateUtc,
            x => x.EndDateUtc,
            x => x.CategoryId
        );

        RuleFor(x => x.DiscountPrice)
            .LessThan(x => x.OriginalPrice)
            .WithMessage("DiscountPrice must be less than OriginalPrice.");
    }
}