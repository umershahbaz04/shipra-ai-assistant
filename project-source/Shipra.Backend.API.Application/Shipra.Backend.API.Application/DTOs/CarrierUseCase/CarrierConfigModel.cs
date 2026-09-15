namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;
public class CarrierConfigModel
{
  public string? Name { get; set; }
  public string? AccountNumber { get; set; }
  public string? UserName { get; set; }
  public string? Password { get; set; }
  public string? Token { get; set; }
  public string? DomainProdURL { get; set; }
  public string? DomainTestURL { get; set; }
  public string? ApiActionURL { get; set; }
  public string? RequestType { get; set; }
  public string? AwbEndPoind { get; set; } = "api/ShipmentAPI/GetAirWayBillLabelsByTrackingNo?TrackingNos";

  public bool? AllowPlacingOrder { get; set; }
  public string? CurrencyCode { get; set; } = "AED";
  public string? RequestId { get; set; } = "3";
}
