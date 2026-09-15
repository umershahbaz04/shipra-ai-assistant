using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;
public class CommonOrderFilters : CommonAddressFilterModel
{ 
  public DateTime? OrderFromDate { get; set; } = null;
  public DateTime? OrderToDate { get; set; } = null;
  public string? StoreId { get; set; } = string.Empty;
  public int? OrderTypeId { get; set; } = 0;
  public string? CarrierId { get; set; } = string.Empty;
  public int? FullFillmentStatusId { get; set; } = 0;
  public int? PaymentStatusId { get; set; } = 0;
  public int? OrderRequestVia { get; set; } = 0;
  public int? PaymentMethodId { get; set; } = 0;
  public string? StationId { get; set; } = string.Empty;
  public bool ReadyForAssignment { get; set; } = false;
  public int CarrierAssign { get; set; } = 0;
  public string? SaleChannelConfigIds { get; set; }
  public string? SalePersonIds { get; set; } 
  public string? CityIds { get; set; } = string.Empty;
  public string? ProvinceIds { get; set; } = string.Empty;
  public string? StateIds { get; set; } = string.Empty;
  public string? PinCodeIds { get; set; } = string.Empty;
  public string? AreaIds { get; set; } = string.Empty;
  public string? CarrierTrackingStatusIds { get; set; } = string.Empty;
  public bool? IncludeRegion { get; set; } = true;
  public bool? IncludeCity { get; set; } = true;
  public string? OrderLabels { get; set; }
  public bool? IsWithoutStation { get; set; } = false;
}
