namespace DiscountsSystem.Application.DTOs.Settings;

public sealed record UpdateSettingsRequest(
    int ReservationDurationMinutes,
    int MerchantEditWindowHours
);