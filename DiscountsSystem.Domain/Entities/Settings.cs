namespace DiscountsSystem.Domain.Entities;

public sealed class Settings
{
    public int Id { get; set; }
    public int ReservationDurationMinutes { get; set; }
    public int MerchantEditWindowHours { get; set; }

}
