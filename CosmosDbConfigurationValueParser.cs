using System.Globalization;

namespace davhdavh.Extensions.Configuration.CosmosDB;

internal sealed class CosmosDbConfigurationValueParser
{
   private readonly Dictionary<string, string?> _data = new(StringComparer.OrdinalIgnoreCase);
   private readonly Stack<string> _paths = new();

   private CosmosDbConfigurationValueParser()
   {
   }

   public static IDictionary<string, string?> Parse(JsonElement element)
   {
      var jsonConfigurationFileParser = new CosmosDbConfigurationValueParser();
      jsonConfigurationFileParser.VisitObjectElement(element);
      return jsonConfigurationFileParser._data;
   }

   private void VisitObjectElement(JsonElement element)
   {
      var isEmpty = true;

      foreach (var property in element.EnumerateObject())
      {
         isEmpty = false;
         EnterContext(property.Name);
         VisitValue(property.Value);
         ExitContext();
      }

      SetNullIfElementIsEmpty(isEmpty);
   }

   private void VisitArrayElement(JsonElement element)
   {
      var index = 0;

      foreach (var arrayElement in element.EnumerateArray())
      {
         EnterContext(index.ToString(CultureInfo.InvariantCulture));
         VisitValue(arrayElement);
         ExitContext();
         index++;
      }

      SetNullIfElementIsEmpty(index == 0);
   }

   private void SetNullIfElementIsEmpty(bool isEmpty)
   {
      if (isEmpty && _paths.Count > 0) _data[_paths.Peek()] = null;
   }

   private void VisitValue(JsonElement value)
   {
      switch (value.ValueKind)
      {
         case JsonValueKind.Object:
            VisitObjectElement(value);
            break;

         case JsonValueKind.Array:
            VisitArrayElement(value);
            break;

         case JsonValueKind.Number:
         case JsonValueKind.String:
         case JsonValueKind.True:
         case JsonValueKind.False:
         case JsonValueKind.Null:
            var key = _paths.Peek();
            if (_data.ContainsKey(key)) throw new FormatException($"Duplicate key {key}");
            _data[key] = value.ToString();
            break;

         default:
            throw new FormatException($"Unsupported json token {value.ValueKind}");
      }
   }

   private void EnterContext(string context) =>
      _paths.Push(_paths.Count > 0 ? _paths.Peek() + ConfigurationPath.KeyDelimiter + context : context);

   private void ExitContext() => _paths.Pop();
}