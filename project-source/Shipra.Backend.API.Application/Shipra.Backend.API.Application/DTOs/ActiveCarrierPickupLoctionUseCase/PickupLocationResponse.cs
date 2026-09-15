using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.ActiveCarrierPickupLoctionUseCase;
public class PickupLocationResponseModel
{
  public int ActiveCarrierPickupLocationId { get; set; }
  public string? ClientId { get; set; }
  public int ActiveCarrierId { get; set; }
  public int CarrierId { get; set; }
  public string? LocationName { get; set; }
  public string? phone { get; set; }
  public string? CustomerServiceNo { get; set; }
  public bool? IsDispatchExCompany { get; set; }

  public CarrierPickupAddressModel? address { get; set; }
}
public class CarrierPickupAddressModel
{
  public int addressId { get; set; }
  public int country { get; set; }
  public string? city { get; set; }
  public string? area { get; set; }
  public string? streetAddress { get; set; }
  public string? streetAddress2 { get; set; }
  public string? houseNo { get; set; }
  public string? buildingName { get; set; }
  public string? landmark { get; set; }
  public string? province { get; set; }
  public string? pinCode { get; set; }
  public string? state { get; set; }
  public string? fullAddress { get; set; }
  public string? zip { get; set; }
  public decimal? latitude { get; set; }
  public decimal? longitude { get; set; }
}
