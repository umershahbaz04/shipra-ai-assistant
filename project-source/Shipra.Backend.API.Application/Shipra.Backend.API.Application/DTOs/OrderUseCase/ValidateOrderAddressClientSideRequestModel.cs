using Shipra.Backend.API.Application.DTOs.CarrierUseCase;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;

public class ValidateOrderAddressClientSideRequestModel
{
  public int CarrierId { get; set; }
  public int? ActiveCarrierId { get; set; }
  public List<ValidateCarrierOrderAddressClientSideModel>? OrderAddress { get; set; } = new(); 
}
