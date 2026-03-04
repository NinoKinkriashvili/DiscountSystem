using DiscountsSystem.Application.DTOs.Settings;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Settings;

public sealed class UpdateSettingsRequestValidator : AbstractValidator<UpdateSettingsRequest>
{
    public UpdateSettingsRequestValidator()
    {
        RuleFor(x => x.ReservationDurationMinutes)
            .InclusiveBetween(1, 1440)
            .WithMessage("Reservation duration must be between 1 and 1440 minutes.");

        RuleFor(x => x.MerchantEditWindowHours)
            .InclusiveBetween(1, 168)
            .WithMessage("Merchant edit window must be between 1 and 168 hours.");
    }
}