using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers;

namespace Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;
public class CreateUpdateClientMetaFieldCommand : IRequest<ServiceResultDTO>
{
  public List<ClientMetaFieldEntry>? ClientMetaFields { get; set; }
}
public class ClientMetaFieldEntry
{
  public int? clientMetaFieldId { get; set; }
  public int? entityMetaFieldId { get; set; }
  public List<SettingConfigItem>? settingConfig { get; set; }
}
public class SettingConfigItem
{
  public string? name { get; set; }
  public string? description { get; set; }
  public FieldType? type { get; set; }
  public List<SelectOption>? selectOptions { get; set; }
  public bool? defaultValue { get; set; }

  [JsonConverter(typeof(ValueJsonConverter))]
  public object? value { get; set; }
}
public class FieldType
{
  public int? id { get; set; }
  public string? label { get; set; }
}
public class SelectOption
{
  public string? id { get; set; }
  public string? label { get; set; }
}
