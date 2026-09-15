using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UpdateProduct;
public class UpdateProductCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
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
  public List<ProductMediaRequestModel>? ProductMedias { get; set; } = new();

  // public ProductOptionReuqestModel? ProductOption { get; set; }
  public List<ProductOptionReuqestModel>? ProductOptions { get; set; } = new();
  public List<ProductStockReuqestModel>? ProductStocks { get; set; } = new();
}
