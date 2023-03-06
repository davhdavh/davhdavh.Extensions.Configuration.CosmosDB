namespace davhdavh.Extensions.Configuration.CosmosDB;

internal class CosmosDbConfigurationProvider : ConfigurationProvider, IConfigurationSource, ICosmosConfigurationSourceRefresher
{
 private readonly Container _container;
 private readonly string _key;
 private readonly PartitionKey _partitionKey;
 private readonly Func<JsonElement, JsonElement> _picker;

 private string _latestEtag = "";

 public CosmosDbConfigurationProvider(CosmosDbConfigurationOptions options)
 {
  _key = options.DocumentId;
  _partitionKey = options.PartitionKey;
  _container = options.Container;
  _picker = options.SubKeyPicker ?? DefaultPicker;
 }

 public IConfigurationProvider Build(IConfigurationBuilder builder)
 {
  return this;
 }

 public async Task<bool> Reload(CancellationToken cancellationToken = default)
 {
  using var response =
   await _container.ReadItemStreamAsync(_key, _partitionKey,
    new ItemRequestOptions { IfNoneMatchEtag = _latestEtag }, cancellationToken).ConfigureAwait(false);
  //just return false when the doc doesn't exist, or was not modified 
  if (!response.IsSuccessStatusCode)
   return false;
  var json = await JsonSerializer.DeserializeAsync<JsonElement>(response.Content, cancellationToken: cancellationToken).ConfigureAwait(false);
  var rootElement = _picker(json);
  Data = CosmosDbConfigurationValueParser.Parse(rootElement);
  _latestEtag = response.Headers.ETag;
  OnReload();
  return true;
 }

 private static JsonElement DefaultPicker(JsonElement e)
 {
  return e;
 }

 public override void Load()
 {
  Reload().GetAwaiter().GetResult();
 }
}