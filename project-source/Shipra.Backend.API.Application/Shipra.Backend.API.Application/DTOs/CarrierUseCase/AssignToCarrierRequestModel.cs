using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;
public class AssignToCarrierRequestModel
{
  public int CarrierId { get; set; }
  public int ActiveCarrierId { get; set; }
  public List<AssignCarrierListRequestModel>? orderList { get; set; }
  public int CarrierContractTypeId { get; set; } = (int)EnumCarrierContractType.OwnContractType;
  public int? ShipraContractCarrierId { get; set; } = 0; // in case of future use
  public bool? CheckPikupLocation { get; set; }
  public bool? IsDispatchEx { get; set; }
  public decimal? DeliveryCharges { get; set; }
}
public class AssignToCarrierOrderInfoRequestModel
{
  public int CarrierId { get; set; }
  public int ActiveCarrierId { get; set; }
  public int? ActiveCarrierPickupLocationId { get; set; }
  public string? OrderNo { get; set; }
  public bool? CheckPikupLocation { get; set; } = false;
  public string? ServiceType { get; set; }
  public Dictionary<string, string>? Others { get; set; }
}
