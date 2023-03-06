using davhdavh.Extensions.Configuration.CosmosDB;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

/// <summary>
/// Extensions to setup cosmos db configuration store
/// </summary>
public static class CosmosDbConfigurationExtensions
{
   /// <summary>
   ///    Adds json data from an Azure Cosmos Database to a configuration builder.
   ///    This uses <see cref="System.Text.Json" /> for serialization, no matter what CosmosDB is configured to use
   /// </summary>
   /// <param name="configurationBuilder">The configuration builder to add configuration store to.</param>
   /// <param name="container">The container that contains the configuration store.</param>
   /// <param name="documentId">Document to lookup</param>
   /// <param name="partitionKey">Using partition key</param>
   /// <returns>The provided configuration builder.</returns>
   public static ICosmosConfigurationSourceRefresher AddCosmosDbConfiguration(
      this IConfigurationBuilder configurationBuilder,
      Container container, string documentId, string? partitionKey = null)
   {
      if (configurationBuilder == null) throw new ArgumentNullException(nameof(configurationBuilder));
      if (container == null) throw new ArgumentNullException(nameof(container));
      if (documentId == null) throw new ArgumentNullException(nameof(documentId));
      return configurationBuilder.AddCosmosDbConfiguration(options =>
         options.SetContainer(container).SetDocumentId(documentId).SetPartitionKey(partitionKey));
   }

   /// <summary>
   ///    Adds json data from an Azure Cosmos Database to a configuration builder.
   ///    This uses <see cref="System.Text.Json" /> for serialization, no matter what CosmosDB is configured to use
   /// </summary>
   /// <param name="configurationBuilder">The configuration builder to add configuration store to.</param>
   /// <param name="action">Configurator function</param>
   /// <returns>The provided configuration builder.</returns>
   public static ICosmosConfigurationSourceRefresher AddCosmosDbConfiguration(
      this IConfigurationBuilder configurationBuilder,
      Action<CosmosDbConfigurationOptions> action)
   {
      if (configurationBuilder == null) throw new ArgumentNullException(nameof(configurationBuilder));
      if (action == null) throw new ArgumentNullException(nameof(action));
      var options = new CosmosDbConfigurationOptions();
      action(options);
      var provider = new CosmosDbConfigurationProvider(options);
      configurationBuilder.Add(provider);
      return provider;
   }

   /// <summary>
   ///    Adds Cosmos DB Configuration services to the specified
   ///    <see cref="IServiceCollection" />.
   ///    <see cref="ICosmosConfigurationSourceRefresher" />.
   /// </summary>
   /// <param name="services">
   ///    The <see cref="IServiceCollection" /> to add services
   ///    to.
   /// </param>
   /// <param name="refresher">
   ///    The refresher from calling
   ///    <see cref="AddCosmosDbConfiguration(IConfigurationBuilder, Container, string, string)" /> or
   ///    <see cref="AddCosmosDbConfiguration(IConfigurationBuilder, Action&lt;CosmosDbConfigurationOptions&gt;)"/>
   /// </param>
   /// <returns>
   ///    The <see cref="IServiceCollection" /> so that additional calls can
   ///    be chained.
   /// </returns>
   public static IServiceCollection AddCosmosDbConfiguration(this IServiceCollection services,
      ICosmosConfigurationSourceRefresher refresher)
   {
      if (services == null)
         throw new ArgumentNullException(nameof(services));
      services.AddSingleton(refresher);
      return services;
   }
}