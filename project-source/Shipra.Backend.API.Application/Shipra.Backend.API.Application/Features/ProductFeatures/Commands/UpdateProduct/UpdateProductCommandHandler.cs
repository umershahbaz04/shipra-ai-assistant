using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UpdateProduct;
public class UpdateProductCommandHandler : RequestHandlerBase<UpdateProductCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IStoreRepository _storeRepository;
  private readonly IProductRepository _productRepository;

  public UpdateProductCommandHandler(IStoreRepository storeRepository, IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<UpdateProductCommandHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(UpdateProductCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
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
      if (product.Sku!.Trim() != request.Sku.Trim()) //check if already exist product
      {
        var existedProduct = await _productRepository.GetProductBySKUAsync(request.Sku, _currentUser.ClientId!);
        if (existedProduct is not null)
        {
          response.Errors?.Add("ProductAlreadyExist", new string[] { "Product already exist against SKU: " + request.Sku });
          response.IsSuccess = false;
          return response;
        }
      }

      request!.QuantityAvailable = request!.ProductStocks?.Sum(x => x.QuantityAvailable).GetValueOrDefault();

      product.UpdateProduct(request?.Sku, request?.ProductName, request?.Price, request?.PurchasePrice, request?.Description, request?.ProductCategoryId, request?.FeatureImage, request?.Weight, request?.HaveOptions, request?.ProductStocks!.Count, request?.QuantityAvailable, _currentUser.ClientId!, request?.StoreId, _currentUser.EmployeeId!);
      #region create product media
      foreach (var pMedia in request?.ProductMedias!)
      {
        if (pMedia.ProductMediaId > 0)
        {
          var productMedia = await _productRepository.GetProductMediaById(pMedia.ProductMediaId);
          if (productMedia is not null)
          {
            productMedia.UpdateGalleryImage(pMedia.ImageGalleryId,product.ProductId, _currentUser.EmployeeId!);
            await _productRepository.UpdateProductMedia(productMedia);
          }
        }
        else
        { 
          await _productRepository.CreateProductMedia(ProductMedia.CreateProductMedia(pMedia.ImageGalleryId, product.ProductId, pMedia.IsFeatured,_currentUser.EmployeeId!));
        }
      }
      #endregion
      #region product options
      var productOptions = await _productRepository.GetProductOptionByProductIdAsync(productId);
      var options = new List<ProductOption>();
      foreach (var item in request?.ProductOptions!)
      {
        hasGUID = Guid.TryParse(item.ProductOptionsId!, out guidID);
        if (!string.IsNullOrEmpty(item.ProductOptionsId) && hasGUID)
        {
          var option = productOptions?.FirstOrDefault(x => x.ProductOptionsId == new ProductOptionsId(new Guid(item.ProductOptionsId!)));
          if (option is null)
          {
            throw new EntityNotFoundException("ProductOption", productId.Value);
          }
          option?.UpdateOption(option.ProductOptionsId, productId, item.OptionId, item.OptionValue, item.DisplayOrder, item.IsDeleted);
        }
        else
        {
          var option = ProductOption.CreateProductOption(productId, item.OptionId, item.OptionValue, item.DisplayOrder);
          var addedOption = await _productRepository.CreateProductOptionAsync(option);
        }
      }

      #endregion
      #region product stocks
      var productStocks = await _productRepository.GetProductStockByProductIdAsync(productId);

      var productStockList = new List<ProductStock>();
      foreach (var item in request?.ProductStocks!)
      {
        //hasGUID = Guid.TryParse(item.ProductStockId!, out guidID);
        if (item.ProductStockId > 0)
        {
          var stock = productStocks?.FirstOrDefault(x => x.ProductStockId == item.ProductStockId);//new ProductStockId(new Guid(item.ProductStockId!)));
          if (stock is null)
          {
            throw new EntityNotFoundException("ProductStock", productId.Value);
          }

          stock?.UpdateProductStock(item.Sku!, item.Price, item.QuantityAvailable, item.LowQuantityLimit, item.ProductStationId, item.VarientOption, item.ProductStockStatusId, item.Active,item.ImageGalleryId);
          //stock?.UpdateLoWQuantityLimit(item.QuantityAvailable, item.LowQuantityLimit);

          dynamic addedStock = await _productRepository.UpdateProductStockAsync(stock!);
        }
        else
        {
          var productStock = ProductStock.CreateProductStock(productId, item.Sku, item.Price, item.QuantityAvailable, item.LowQuantityLimit, item.ProductStationId, item.VarientOption, _currentUser.EmployeeId!,item.ImageGalleryId);
          ProductStock data = await _productRepository.CreateProductStockAsync(productStock);
        }
      }
      #endregion
      var result = await _productRepository.UpdateProductAsync(product, productOptions);
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
