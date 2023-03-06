namespace davhdavh.Extensions.Configuration.CosmosDB;

/// <summary>
/// Configuration object for the CosmosDbConfigurationProvider. This object does not get registered as a proper options object (ie, so it can't be injected as IOptions&lt;CosmosDbConfigurationProvider&gt;)
/// </summary>
public class CosmosDbConfigurationOptions
{
   /// <summary>
   /// Get/Set the Container used to make the lookup. Required.
   /// </summary>
   public Container Container { get; set; } = null!;
   /// <summary>
   /// Get/Set which DocumentId to lookup. Required.
   /// </summary>
   public string DocumentId { get; set; } = null!;
   /// <summary>
   /// Get/Set which PartitionKey to use. Default use PartitionKey.Null.
   /// </summary>
   public PartitionKey PartitionKey { get; set; } = PartitionKey.Null;
   /// <summary>
   /// Get/Set the function used to find the right root key in the document.
   /// </summary>
   /// <example>
   ///.SetSubkeyPicker(e => e.GetProperty("here").GetProperty("there"))
   /// </example>
   public Func<JsonElement, JsonElement>? SubKeyPicker { get; set; }

}

/// <summary>
/// Extensions to set parameters in <see cref="CosmosDbConfigurationOptions"/>
/// </summary>
public static class CosmosDbConfigurationOptionsExtensions
{
   /// <summary>
   /// Set the Container to use
   /// </summary>
   /// <example>
   ///.SetContainer(new CosmosClient(...).GetContainer("db", "container")))
   /// </example>
   /// <returns>The <see cref="CosmosDbConfigurationOptions" /> so that additional calls can be chained.</returns>
   public static CosmosDbConfigurationOptions SetContainer(this CosmosDbConfigurationOptions options,
      Container container)
   {
      if (options == null) throw new ArgumentNullException(nameof(options));
      options.Container = container;
      return options;
   }

   /// <summary>
   /// Set the DocumentId to lookup the json in
   /// </summary>
   /// <example>
   ///.SetDocumentId("MyAppId")
   /// </example>
   /// <returns>The <see cref="CosmosDbConfigurationOptions" /> so that additional calls can be chained.</returns>
   public static CosmosDbConfigurationOptions SetDocumentId(this CosmosDbConfigurationOptions options, string id)
   {
      if (options == null) throw new ArgumentNullException(nameof(options));
      options.DocumentId = id ?? throw new ArgumentNullException(nameof(id));
      return options;
   }

   /// <summary>
   /// Set the PartitionKey to use
   /// </summary>
   /// <example>
   ///.SetPartitionKey("Configuration")
   /// </example>
   /// <returns>The <see cref="CosmosDbConfigurationOptions" /> so that additional calls can be chained.</returns>
   public static CosmosDbConfigurationOptions SetPartitionKey(this CosmosDbConfigurationOptions options, string? key)
   {
      if (options == null) throw new ArgumentNullException(nameof(options));
      options.PartitionKey = key == null ? PartitionKey.Null : new(key);
      return options;
   }

   /// <summary>
   /// Set the PartitionKey to use
   /// </summary>
   /// <example>
   ///.SetPartitionKey(PartitionKey.Null)
   /// </example>
   /// <returns>The <see cref="CosmosDbConfigurationOptions" /> so that additional calls can be chained.</returns>
   public static CosmosDbConfigurationOptions SetPartitionKey(this CosmosDbConfigurationOptions options, PartitionKey key)
   {
    if (options == null) throw new ArgumentNullException(nameof(options));
    options.PartitionKey = key;
      return options;
   }

   /// <summary>
   /// Get/Set the function used to find the right root key in the document.
   /// </summary>
   /// <example>
   ///.SetSubkeyPicker(e => e.GetProperty("here").GetProperty("there"))
   /// </example>
   /// <returns>The <see cref="CosmosDbConfigurationOptions" /> so that additional calls can be chained.</returns>
   public static CosmosDbConfigurationOptions SetSubkeyPicker(this CosmosDbConfigurationOptions options, Func<JsonElement, JsonElement>? subKeyPicker)
   {
    if (options == null) throw new ArgumentNullException(nameof(options));
    options.SubKeyPicker = subKeyPicker;
    return options;
   }
}