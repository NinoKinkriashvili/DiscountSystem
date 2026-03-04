using DiscountsSystem.Application.DTOs.Offers;
using DiscountsSystem.Domain.Enums;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Offers;

public sealed class UpdateOfferStatusRequestValidator : AbstractValidator<UpdateOfferStatusRequest>
{
    public UpdateOfferStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.")
            .Must(BeValidModerationStatus)
            .WithMessage("Status must be Approved or Rejected.");

        When(x => IsRejected(x.Status), () =>
        {
            RuleFor(x => x.RejectReason)
                .NotEmpty()
                .WithMessage("Reject reason is required when rejecting.")
                .MaximumLength(500);
        });

        When(x => IsApproved(x.Status), () =>
        {
            RuleFor(x => x.RejectReason)
                .Must(string.IsNullOrWhiteSpace)
                .WithMessage("Reject reason must be empty when approving.");
        });
    }

    private static bool BeValidModerationStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return false;

        return Enum.TryParse<OfferStatus>(status, ignoreCase: true, out var parsed)
               && parsed is OfferStatus.Approved or OfferStatus.Rejected;
    }

    private static bool IsRejected(string? status)
        => Enum.TryParse<OfferStatus>(status, ignoreCase: true, out var parsed)
           && parsed == OfferStatus.Rejected;

    private static bool IsApproved(string? status)
        => Enum.TryParse<OfferStatus>(status, ignoreCase: true, out var parsed)
           && parsed == OfferStatus.Approved;
}