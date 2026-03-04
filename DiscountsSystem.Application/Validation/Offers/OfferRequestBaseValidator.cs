using System.Linq.Expressions;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Offers;

public abstract class OfferRequestBaseValidator<T> : AbstractValidator<T>
    where T : class
{
    protected void ApplyCommonRules(
        Expression<Func<T, string>> title,
        Expression<Func<T, string>> description,
        Expression<Func<T, decimal>> originalPrice,
        Expression<Func<T, decimal>> discountPrice,
        Expression<Func<T, int>> couponQuantityTotal,
        Expression<Func<T, DateTime>> startDateUtc,
        Expression<Func<T, DateTime>> endDateUtc,
        Expression<Func<T, int>> categoryId)
    {
        RuleFor(title)
            .NotEmpty()
            .Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(description)
            .NotEmpty()
            .Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage("Description is required.")
            .MaximumLength(2000);

        RuleFor(originalPrice)
            .GreaterThan(0);

        RuleFor(discountPrice)
            .GreaterThan(0);

        RuleFor(couponQuantityTotal)
            .GreaterThan(0);

        RuleFor(startDateUtc)
            .Must(d => d != default)
            .WithMessage("StartDateUtc is required.");

        RuleFor(endDateUtc)
            .Must(d => d != default)
            .WithMessage("EndDateUtc is required.");

        RuleFor(startDateUtc)
            .LessThan(endDateUtc)
            .WithMessage("StartDateUtc must be less than EndDateUtc.");

        RuleFor(categoryId)
            .GreaterThan(0);
    }
}
