using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductById;
public class GetProductByIdQuery : IRequest<ServiceResultDTOWithTypeModel<ProductResponseModel>>
{
  public string? ProductId { get; set; }
}
public class GetProductByIdQueryHandler : RequestHandlerBase<GetProductByIdQuery, ServiceResultDTOWithTypeModel<ProductResponseModel>>
{
  private readonly IProductRepository _productRepository;

  public GetProductByIdQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetProductByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ProductResponseModel>> HandleRequest(GetProductByIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<ProductResponseModel>();
    try
    {
      Guid guidID;
      var hasGUID = Guid.TryParse(request!.ProductId!, out guidID);
      if (!hasGUID)
      {
        throw new InvalidIdTypeException(request!.ProductId!);
      }
      var productId = new ProductId(new Guid(request!.ProductId!));

      var product = await _productRepository.GetProductByIdAsync(productId, _currentUser.ClientId!);
      if (product is null)
      {
        throw new EntityNotFoundException("Product", productId.Value);
      }

      var productDTO = _mapper.Map<ProductResponseModel>(product);

      var productOptions = await _productRepository.GetProductOptionByProductIdAsync(productId);
      var productVariantsAndBalances = await _productRepository.GetProductVariantsAndBalancesAsync(productId, _currentUser.ClientId!.Value.ToString());
      var productMedias = await _productRepository.GetProductMediaByProductId(productId);
       
      var productOptionMapping = _mapper.Map<List<ProductOptionReuqestModel>>(productOptions);
      var productStockMapping = new List<ProductStockReuqestModel>();
      foreach (var row in productVariantsAndBalances)
      {
          productStockMapping.Add(new ProductStockReuqestModel {
              InventoryBalanceId = row.InventoryBalanceId != null ? (long?)row.InventoryBalanceId : null,
              ProductVariantId = row.ProductVariantId != null ? (long?)row.ProductVariantId : null,
              ProductStockId = row.InventoryBalanceId != null ? (long)row.InventoryBalanceId : (row.ProductVariantId != null ? (long)row.ProductVariantId : 0),
              ProductId = productId.Value.ToString(),
              Sku = row.SKU,
              Price = row.Price != null ? (decimal?)row.Price : null,
              QuantityAvailable = row.QuantityAvailable != null ? (int?)row.QuantityAvailable : null,
              LowQuantityLimit = row.LowQuantityLimit != null ? (int)row.LowQuantityLimit : 0,
              ProductStationId = row.ProductStationId != null ? (int?)row.ProductStationId : null,
              VarientOption = row.VarientOption,
              Active = row.Active != null ? Convert.ToBoolean(row.Active) : false,
              ProductVariantStatusId = row.ProductVariantStatusId != null ? (int?)row.ProductVariantStatusId : null,
              ProductStockStatusId = row.ProductVariantStatusId != null ? (int?)row.ProductVariantStatusId : null,
              ImageGalleryId = row.ImageGalleryId != null ? (long?)row.ImageGalleryId : null,
              VariantAttributes = new ProductVariantRequestModel {
                  Barcode = row.Barcode,
                  Weight = row.Weight != null ? (decimal?)row.Weight : null,
                  Length = row.Length != null ? (decimal?)row.Length : null,
                  Width = row.Width != null ? (decimal?)row.Width : null,
                  Height = row.Height != null ? (decimal?)row.Height : null
              }
          });
      }
      var productMediaMapping = _mapper.Map<List<ProductMediaResponseModal>>(productMedias);
      if (productStockMapping != null && productStockMapping.Count > 0)
      {
        var allImages = await _productRepository.GetAllImageGalleries(_currentUser.ClientId!);

        foreach (var item in productStockMapping!)
        {
          item.ImageUrl = allImages.Where(x => x.ImageGalleryId == item.ImageGalleryId).Select(x => x.ImageUrl).FirstOrDefault();
        }
      }
      productDTO.ProductOptions = productOptionMapping;
      productDTO.ProductStocks = productStockMapping;
      productDTO.ProductMedias = productMediaMapping;
      serviceResult = new ServiceResultDTOWithTypeModel<ProductResponseModel>(productDTO);

      serviceResult.CreateSuccessResponse();
      return serviceResult;

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
