using Shipra.Backend.API.Application.DTOs.OrderUseCase;

namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;

public class PickupLocationAddressClientSideModel : PickupLocationAddressModel
{
  public new string? CityId { get; set; }
  public new string? AreaId { get; set; }  // This is a string in client-side request  
  public new string? ProvinceId { get; set; }
  public new string? PinCodeId { get; set; }
  public new string? StateId { get; set; }
   
}
public class PickupLocationAddressModel : OrderAddressModel
{
  
  public string? CustomerServiceNo { get; set; } 
  public int? CarrierId { get; set; }
  public int? ActiveCarrierId { get; set; }
  public string? LocationName { get; set; }
  public string? Phone { get; set; }
  public int ActiveCarrierPickupLocationId { get; set; }
}



public class ValidateCarrierOrderAddressClientSideModel : OrderAddressModel
{
  public string? TrackingNo { get; set; }
  public new string? CityId { get; set; }
  public new string? AreaId { get; set; }  // This is a string in client-side request  
  public new string? ProvinceId { get; set; }
  public new string? PinCodeId { get; set; }
  public new string? StateId { get; set; }
}
 
