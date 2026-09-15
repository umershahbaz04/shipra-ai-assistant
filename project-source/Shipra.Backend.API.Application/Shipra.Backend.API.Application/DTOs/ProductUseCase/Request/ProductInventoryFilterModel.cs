using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
public class ProductInventoryFilterModel : CommonFilterModel
{
  public string? StoreId { get; set; }
  public int? ProductStationId { get; set; }
  public bool? IsActive { get; set; }
  public bool? IsAvailable { get; set; }
  public int? AvailableQty { get; set; } 
}
