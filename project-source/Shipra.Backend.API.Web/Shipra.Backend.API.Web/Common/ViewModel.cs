using System;
using Newtonsoft.Json;

namespace Shipra.Backend.API.Web.Common;

public class ViewModel
{
  [JsonProperty("metadata")]
  public Metadata? Metadata { get; set; }
}
