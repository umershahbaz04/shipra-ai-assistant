using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shipra.Backend.API.Application.Helpers;

public class ValueJsonConverter : JsonConverter
{
  public override bool CanConvert(Type objectType)
  {
    return true;
  }

  public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
  {
    var token = JToken.Load(reader);

    return token.Type switch
    {
      JTokenType.Object => token.ToObject<Dictionary<string, object>>(serializer),
      JTokenType.Array => token.ToObject<List<object>>(serializer),
      JTokenType.Integer => token.ToObject<int>(serializer),
      JTokenType.Float => token.ToObject<float>(serializer),
      JTokenType.Boolean => token.ToObject<bool>(serializer),
      JTokenType.String => token.ToString(),
      _ => null
    };
  }

  public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
  {
    JToken.FromObject(value ?? "", serializer).WriteTo(writer);
  }

  public override bool CanRead => true;
  public override bool CanWrite => true;
}

