namespace Shipra.Backend.API.Core.SaleChannelConfigAggregate;
public class SaleChannelLookup
{
  /// <summary>
  /// PaymentProcessId
  /// </summary>
  public int SaleChannelLookupId { get; set; }
  public string? SaleChannelName { get; set; }
  public string? InputRequiredConfig { get; set; }
  public string? ImageUrl { get; set; }
  public string? SettingConfig { get; set; }
  public string? AppUrl { get; set; }
}
