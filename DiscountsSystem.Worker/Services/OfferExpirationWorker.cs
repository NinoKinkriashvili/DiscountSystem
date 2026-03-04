using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Worker.Options;
using Microsoft.Extensions.Options;

namespace DiscountsSystem.Worker.Services;

public sealed class OfferExpirationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OfferExpirationWorker> _logger;
    private readonly WorkerOptions _options;

    public OfferExpirationWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<OfferExpirationWorker> logger,
        IOptions<WorkerOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "OfferExpirationWorker started. IntervalSeconds = {IntervalSeconds}",
            _options.IntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var offerRepository =
                    scope.ServiceProvider.GetRequiredService<IOfferRepository>();

                var timeProvider =
                    scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

                var nowUtc = timeProvider.UtcNow;

                var expiredCount = await offerRepository.ExpireDueAsync(nowUtc, stoppingToken);

                _logger.LogInformation(
                    "Offer expiration executed at {UtcNow}. Expired offers: {ExpiredCount}",
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
                _logger.LogError(ex, "OfferExpirationWorker failed during execution cycle.");
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

        _logger.LogInformation("OfferExpirationWorker stopped.");
    }
}