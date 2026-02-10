using Microsoft.EntityFrameworkCore;

namespace HQS_SWS;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scraper started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var scraper = new WaitTimeScraper(_logger);
                var result = await scraper.ScrapeAsync(stoppingToken);

                if (result != null && result.Any())
                {
                    _logger.LogInformation("Count: {result.Count()}", result.Count());

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    foreach (var r in result)
                    {
                        // Console.WriteLine($"{r.HospitalName} : {r.Waittime} -> {TimeParser.ParseToMinutes(r.Waittime)}");
                        var entity = new ScrapedData
                        {
                            HospitalName = r.HospitalName,
                            WaitTimeMinutes = TimeParser.ParseToMinutes(r.Waittime),
                            ScrapedAtUtc = DateTime.UtcNow
                        };
                        await CheckAndSaveToDb(db, entity);
                    }
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Scraped data saved to database");
                }
                else
                {
                    _logger.LogWarning("Scraping returned no result");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scraping failed");
            }

            // ⏱ Run every 10 minutes
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            _logger.LogInformation("...run again...");
        }
    }

    async Task CheckAndSaveToDb(AppDbContext context, ScrapedData data)
    {
        var existing = await context.ScrapedHospitalData
                            .FirstOrDefaultAsync(x => x.HospitalName == data.HospitalName);

        if(existing != null)
        {
            existing.WaitTimeMinutes = data.WaitTimeMinutes;
            existing.ScrapedAtUtc = data.ScrapedAtUtc;
            context.ScrapedHospitalData.Update(existing);
        }
        else
        {
            context.ScrapedHospitalData.Add(data);
        }
        await context.SaveChangesAsync();
    }
}
