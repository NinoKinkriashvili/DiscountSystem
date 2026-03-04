using DiscountsSystem.Application.Interfaces.Common;

namespace DiscountsSystem.Infrastructure.Services.Common;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}