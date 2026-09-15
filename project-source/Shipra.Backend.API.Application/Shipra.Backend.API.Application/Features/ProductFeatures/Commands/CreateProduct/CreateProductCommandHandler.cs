using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateProduct;
public class CreateProductCommandHandler : RequestHandlerBase<CreateProductCommand, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;
  private readonly IProductRepository _productRepository;

  public CreateProductCommandHandler(IStoreRepository storeRepository, IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<CreateProductCommandHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateProductCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    { 
      ///    <summary>
      /// 1. checkForProductSku if there was not productsku in product table
      ///    as given from frontend then add or create this product in product table 
      ///    else throw an exception that productsku already exists
      /// 2. add or create the productstocks or list of productstocks 
      ///    in productstock table entered from frontend
      /// 3. add or create productstockhistory or list of productstockhistory 
      ///    in productstockhistory table for the entered productstock
      /// 4. add or create the productoptions or list of productoptions 
      ///    in productoptions table entered from frontend
      ///    </summary>
      var productCommon = new ProductCommon();
      var existedProduct = await _productRepository.GetProductBySKUAsync(request.SKU, _currentUser.ClientId!);

      if (existedProduct is null)
      {
        var currencyId = (int)EnumCurrency.AED;//we need this currency from client table when we manage currency 
        var product = GetProduct(request, currencyId, request.ProductStocks!.Count());
        var result = await _productRepository.CreateProductAsync(product);
        if (result is not null)
        {
          if (request.StoreId.GetValueOrDefault() > 0)
          {
            StoreProduct storeProduct = StoreProduct.CreateStoreProduct(request.StoreId, product.ProductId, _currentUser.EmployeeId);
            await _storeRepository.CreateStoreProduct(storeProduct);
          }

          // 1. Create ProductVariants in dbo.ProductVariant
          var oProductVariantList = productCommon.GetProductVariants(request.ProductStocks, product.ProductId, _currentUser.ClientId, product.PurchasePrice, product.Weight, _currentUser.EmployeeId);
          await _productRepository.CreateProductVariantsAsync(oProductVariantList);

          // 2. Create InventoryBalances in dbo.InventoryBalance
          var oInventoryBalanceList = productCommon.GetInventoryBalances(oProductVariantList, request.ProductStocks);
          if (oInventoryBalanceList.Any())
          {
              await _productRepository.CreateInventoryBalancesAsync(oInventoryBalanceList);
          }

          // 3. Create InventoryTransactions in dbo.InventoryTransaction
          if (request.TrackInventory.GetValueOrDefault())
          {
              var oInventoryTransactionList = productCommon.GetInventoryTransactions(oProductVariantList, request.ProductStocks, _currentUser.EmployeeId);
              if (oInventoryTransactionList.Any())
              {
                  await _productRepository.CreateInventoryTransactionsAsync(oInventoryTransactionList);
              }
          }

          #region create product media
          if (request.ProductMedias != null)
          {
              foreach (var pMedia in request.ProductMedias)
              {
                  await _productRepository.CreateProductMedia(ProductMedia.CreateProductMedia(pMedia.ImageGalleryId, product.ProductId, pMedia.IsFeatured, _currentUser.EmployeeId!));
              }
          }
          #endregion

          // 4. Create ProductOptions and link to variants
          var productOptions = productCommon.GetProductOptions(request.ProductOptions, product.ProductId);
          if (productOptions.Any())
          {
              var addedproductOptions = await _productRepository.CreateProductOptionsAsync(productOptions);

              // Map variants to options and save ProductVariantOptions
              var variantOptionsToSave = new List<ProductVariantOption>();
              foreach (var variant in oProductVariantList)
              {
                  if (!string.IsNullOrEmpty(variant.VariantOptionText))
                  {
                      var optionValues = variant.VariantOptionText.Split(new[] { " / ", "/" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                      foreach (var optVal in optionValues)
                      {
                          var matchedOption = productOptions.FirstOrDefault(po => po.OptionValue == optVal);
                          if (matchedOption != null && matchedOption.ProductOptionsId != null)
                          {
                              variantOptionsToSave.Add(ProductVariantOption.Create(variant.ProductVariantId, matchedOption.ProductOptionsId.Value));
                          }
                      }
                  }
              }
              if (variantOptionsToSave.Any())
              {
                  await _productRepository.CreateProductVariantOptionsAsync(variantOptionsToSave);
              }
          }
        }
        response = new ServiceResultDTO(new BaseResponseDto
        {
          Data = new
          {
            ProductId = product.ProductId != null ? product.ProductId.Value.ToString() : "",
            Sku = product.Sku
          },
          Message = NotificationConstants.SavedSuccess
        });

        return response;
      }
      else
      {
        response.Errors?.Add("ProductAlreadyExist", new string[] { "Product already exist against SKU: " + request.SKU });
        response.IsSuccess = false;
        return response;
      }
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }

  #region construct product related model

  private Product GetProduct(CreateProductCommand? model, int currencyId, int varientCount)
  {
    model!.QuantityAvailable = model!.ProductStocks?.Sum(x => x.QuantityAvailable).GetValueOrDefault();
    if (string.IsNullOrEmpty(model.FeatureImage))
    {
      model.FeatureImage = ApplicationConstants.ProductPlaceHolder;
    }

    var productStatusId = (int)EnumProductStockStatus.Active;
    var product = Product.CreateProduct(model?.SKU, model?.ProductName, model?.Price, model?.PurchasePrice, model?.Description, model?.ProductCategoryId, model?.FeatureImage, model?.Weight, model?.HaveOptions, varientCount, model?.QuantityAvailable, _currentUser.ClientId!, productStatusId, model?.TrackInventory, currencyId, _currentUser.EmployeeId!);
    return product;
  }
  #endregion 

}
