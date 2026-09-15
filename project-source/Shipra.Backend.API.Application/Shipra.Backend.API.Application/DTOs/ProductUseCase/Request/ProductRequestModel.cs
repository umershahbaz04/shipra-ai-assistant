using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
public class ProductRequestModel
{
  public string Sku { get; set; } = null!;
  public string? ProductName { get; set; }
  public decimal? Price { get; set; }
  public decimal? PurchasePrice { get; set; }
  public string? Description { get; set; }
  public int? ProductCategoryId { get; set; }
  public string? FeatureImage { get; set; }
  public decimal? Weight { get; set; }
  public bool? HaveOptions { get; set; }
  public int? VarientCount { get; set; }
  public int? QuantityAvailable { get; set; }
  public ClientId? ClientId { get; set; }
  public int? StoreId { get; set; }
  public int? ProductStatusId { get; set; }
  public bool? TrackInventory { get; set; }
  public bool? Active { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }

}
