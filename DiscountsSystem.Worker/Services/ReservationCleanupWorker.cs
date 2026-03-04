using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Worker.Options;
using Microsoft.Extensions.Options;

namespace DiscountsSystem.Worker.Services;

public sealed class ReservationCleanupWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationCleanupWorker> _logger;
    private readonly WorkerOptions _options;

    public ReservationCleanupWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ReservationCleanupWorker> logger,
        IOptions<WorkerOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "ReservationCleanupWorker started. IntervalSeconds = {IntervalSeconds}",
            _options.IntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var reservationRepository =
                    scope.ServiceProvider.GetRequiredService<IReservationRepository>();

                var timeProvider =
                    scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

                var nowUtc = timeProvider.UtcNow;

                var expiredCount = await reservationRepository.ExpireDueAsync(nowUtc, stoppingToken);

                _logger.LogInformation(
                    "Reservation cleanup executed at {UtcNow}. Expired reservations: {ExpiredCount}",
                    nowUtc,
                    expiredCount);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationCleanupWorker failed during execution cycle.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("ReservationCleanupWorker stopped.");
    }
}