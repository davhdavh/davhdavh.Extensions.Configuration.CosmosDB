using Microsoft.Extensions.Hosting;

namespace davhdavh.Extensions.Configuration.CosmosDB;

//public class CosmosDbConfigurationRefresher : BackgroundService
//{
//   private readonly ICosmosConfigurationSourceRefresher _provider;
//   private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(3); //20 times an hour

//   public CosmosDbConfigurationRefresher(ICosmosConfigurationSourceRefresher provider)
//   {
//      _provider = provider;
//   }

//   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//   {
//      var timer = new PeriodicTimer(_refreshInterval);
//      while (await timer.WaitForNextTickAsync(stoppingToken))
//         if (await _provider.Reload(stoppingToken))
//         {
//            //new options available. You don't have to do anything here, the IOptions, IOptionSnapshot, IOptionsMonitor etc has already done all of the work
//         }
//   }
//}