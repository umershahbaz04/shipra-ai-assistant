namespace Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;

public class UDTOrderDetailSimplifiedCommon
{
  public string? CustomerName { get; set; }
  public string? MobileNumber { get; set; }
  public string? StationCode { get; set; }
  public string? MobileNumber2 { get; set; }
  public string? RefNo { get; set; }
  public string? CountryCode { get; set; }
  public string? CityName { get; set; }
  public string? State { get; set; }
  public string? Province { get; set; }
  public string? AreaCode { get; set; }
  public string? PinCode { get; set; }
  public string? StreetAddress { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? PaymentMethod { get; set; }
  public decimal Amount { get; set; }
  public string? Remarks { get; set; }
  public string? BoxName { get; set; }
  public string? Description { get; set; }
  public string? StoreCode { get; set; }
  public string? SalePerson { get; set; }
  public int? NoOfPieces { get; set; } = 1;
}
