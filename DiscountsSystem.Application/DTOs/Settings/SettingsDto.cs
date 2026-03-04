namespace DiscountsSystem.Application.DTOs.Settings;

public sealed record SettingsDto(
    int ReservationDurationMinutes,
    int MerchantEditWindowHours
);