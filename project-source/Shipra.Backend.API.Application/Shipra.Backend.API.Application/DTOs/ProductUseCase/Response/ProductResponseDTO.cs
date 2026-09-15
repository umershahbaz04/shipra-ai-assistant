using System.Diagnostics.SymbolStore;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
public class ProductResponseDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}
public class ProductResponseModel
{
  public string? ProductId { get; set; } 
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
  public int? StoreId { get; set; } 
  public int? ProductStatusId { get; set; } 
  public bool? TrackInventory { get; set; }
  public bool? Active { get; set; } 
  public List<ProductOptionReuqestModel>? ProductOptions { get; set; } = new();
  public List<ProductStockReuqestModel>? ProductStocks { get; set; } = new();
  public List<ProductMediaResponseModal>? ProductMedias { get; set; } = new();
}
public class ExistProductResponseModel
{
  public bool IsExist { get; set; }
  public string? Message { get; set; }
  public string? Sku { get; set; }
}
