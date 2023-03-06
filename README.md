# davhdavh.Extensions.Configuration.CosmosDB
Use cosmosdb as a json source for your configuration

# How to Use the Cosmos DB Configuration API

The Cosmos DB Configuration API is a library that allows you to retrieve configuration data stored in a Cosmos DB container and add it to a .NET Core configuration builder.
This document provides instructions for using the API in your .NET Core or .Net standard 2.0 application.

## Prerequisites

To use the Cosmos DB Configuration API, you need the following:

- A .NET Core or .Net standard 2.0 application
- A Cosmos DB account and a container that contains your configuration data
- The Cosmos DB Configuration API installed in your application

## Installation

You can install the Cosmos DB Configuration API using the NuGet Package Manager or the Package Manager Console in Visual Studio. Use the following command to install the API:

```PowerShell
Install-Package davhdavh.Extensions.Configuration.CosmosDB
```

## Usage

To use the Cosmos DB Configuration API, follow these steps:

1. Create an instance of the `CosmosClient` class and connect it to your Cosmos DB account.
2. Create an instance of the `Container` class and connect it to your container that contains your configuration data.
3. Add the Cosmos DB Configuration API to your `IConfigurationBuilder` by calling the `AddCosmosDbConfiguration` method.
4. Build your configuration by calling the `Build` method on your configuration builder.

### Retrieving Configuration Data

To retrieve configuration data from your Cosmos DB container, use the `AddCosmosDbConfiguration` method. You can call this method in two ways:

- Pass the `IConfigurationBuilder`, `Container`, and `documentId` as parameters:

```csharp
using davhdavh.Extensions.Configuration.CosmosDB;

var client = new CosmosClient("<your connection string>");
var container = client.GetContainer("<your database id>", "<your container id>");
var builder = new ConfigurationBuilder();
var refresher = builder.AddCosmosDbConfiguration(container, "<your document id>");
var configuration = builder.Build();
```

or 

```csharp
using davhdavh.Extensions.Configuration.CosmosDB;
var host = Host.CreateDefaultBuilder();

var client = new CosmosClient("<your connection string>");
var container = client.GetContainer("<your database id>", "<your container id>");

ICosmosConfigurationSourceRefresher? refresher = null;

host.ConfigureAppConfiguration((context, builder) =>
{
	refresher = builder.AddCosmosDbConfiguration(container, "<your document id>", "<your partition key>");
})
.ConfigureServices((hostCtx, services) =>
{
  services.AddCosmosDbConfiguration(refresher!);
});
;
```

### Cosmos DB Configuration Options

The CosmosDbConfigurationOptions object is used to configure the CosmosDbConfigurationProvider. This object allows you to specify the container, document ID, partition key, and subkey picker function used to retrieve the configuration data.

- `Container`(required): The container used to make the lookup.
- `DocumentId`(required): The document ID to lookup.
- `PartitionKey`: The partition key to use. Defaults to PartitionKey.Null.
- `SubKeyPicker`: The function used to find the right root key in the document.

#### Example

```csharp
builder.AddCosmosDbConfiguration(options =>
{
    options.SetCosmosClient("<your connection string>")
           .SetDatabaseId("<your database id>")
           .PartitionKey(new("<your partition key>"))
           .SetDocumentId("<your document id>");
    options.SubKeyPicker = json =>
    {
        if (json.TryGetProperty("root", out var root))
        {
            return root.GetProperty("sub");
        }
        throw new Exception("Subkey not found");
    };
});
```

Both methods will add the configuration data from your Cosmos DB container to your configuration builder.

## Updating Configuration Data

If you need to update your configuration data, you can use the `ICosmosConfigurationSourceRefresher` returned from the `AddCosmosDbConfiguration` method to refresh your configuration.

```csharp
public class CosmosDbConfigurationRefresher : BackgroundService
{
   private readonly ICosmosConfigurationSourceRefresher _provider;
   private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(3); //20 times an hour

   public CosmosDbConfigurationRefresher(ICosmosConfigurationSourceRefresher provider)
   {
      _provider = provider;
   }

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      var timer = new PeriodicTimer(_refreshInterval);
      while (await timer.WaitForNextTickAsync(stoppingToken))
         if (await _provider.Reload(stoppingToken))
         {
            //new options available. You don't have to do anything here, the IOptions, IOptionSnapshot, IOptionsMonitor etc has already done all of the work
         }
   }
}
...
services.AddCosmosDbConfiguration(refresher!);
services.AddHostedService<CosmosDbConfigurationRefresher>();
```

## Warning

Due to the way that configuration works in `IConfiguration` working with arrays can be a bit tricky.

If your appsettings.json (or rather any previous `IConfigurationSource`) contains configuration for something with 2 values, but you in cosmosdb only have 1 value, the cosmosdb configuration will override FIRST item only, the second will remain.

IE:

```json
{
  "MyArray": [
	"first",
	"second"
  ]
}
```

```json
{
  "MyArray": [
	"fromCosmosDb"
  ]
}
```

Will result in

```json
{
  "MyArray": [
	"fromCosmosDb",
	"second"
  ]
}
```