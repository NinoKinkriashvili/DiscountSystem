using DiscountsSystem.Application.DTOs.Categories;
using DiscountsSystem.Application.Validation.Common;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Categories;

public sealed class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Category name is required.")
            .MinimumLength(2).WithMessage("Category name must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Category name must be at most 200 characters.")
            .Must(NameRules.BeLatinNameWithSpaceOrHyphen)
            .WithMessage("Category name must contain only Latin letters and single spaces/hyphens.");
    }
}