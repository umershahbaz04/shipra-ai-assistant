namespace Shipra.Backend.API.Core.Models;

public class GeneralSettingConfigModel
{
  public string? SectionName { get; set; }
  public string? Key { get; set; }
  public bool? IsHide { get; set; }
  public List<GeneralSettingConfigData>? InputData { get; set; } = new();
}
public class GeneralSettingConfigData
{
  public string? Label { get; set; }
  public string? Key { get; set; }
  public string? Type { get; set; }
  public bool? Multiple { get; set; }
  public bool? Required { get; set; }
  public string? Description { get; set; }
  public List<string>? Data { get; set; }
  public string? Value { get; set; }
  public SourceModel? Src { get; set; }
}
public class SourceModel
{
  public string? Url { get; set; }
  public string? Method { get; set; }
  public SourceObj? Obj { get; set; }
}

public class SourceObj
{
  public string? Id { get; set; }
  public string? Label { get; set; }
}
