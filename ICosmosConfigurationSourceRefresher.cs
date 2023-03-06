namespace davhdavh.Extensions.Configuration.CosmosDB;

/// <summary>
/// Object that lets you force a reload from the db.
/// </summary>
/// <example>
/// internal class CosmosConfigurationRefresher : BackgroundService
/// {
///   private readonly ICosmosConfigurationSource _provider;
///   private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(3); //20 times an hour
///   
///   public CosmosConfigurationRefresher(ICosmosConfigurationSource provider) => _provider = provider;
///   
///   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
///   {
///     var timer = new PeriodicTimer(_refreshInterval);
///     while (await timer.WaitForNextTickAsync(stoppingToken))
///     if (await _provider.Reload(stoppingToken))
///       Console.WriteLine("Updated");
///     else
///       Console.WriteLine("Etag matched - no update");
///   }
/// }
/// </example>
public interface ICosmosConfigurationSourceRefresher
{
   /// <summary>
   /// Call to force a reload from the db. This uses ETAGs to ensure no data is transferred if nothing changed
   /// </summary>
   /// <param name="cancellationToken">Cancellation token to cancel db request</param>
   /// <returns>True if changes were detected</returns>
   Task<bool> Reload(CancellationToken cancellationToken);
}