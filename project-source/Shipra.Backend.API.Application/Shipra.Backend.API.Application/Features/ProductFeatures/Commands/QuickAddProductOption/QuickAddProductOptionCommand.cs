using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateProduct;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.QuickAddProductOption;
public class QuickAddProductOptionCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public QuickAddProductStockReuqestModel? ProductStock { get; set; }
}

public class QuickAddProductOptionCommandHandler : RequestHandlerBase<QuickAddProductOptionCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IStationLookupRepository _stationLookupRepository;
  private readonly IProductRepository _productRepository;

  public QuickAddProductOptionCommandHandler(IStationLookupRepository stationLookupRepository, IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<QuickAddProductOptionCommandHandler> logger) : base(serviceProvider, logger)
  {
    _stationLookupRepository = stationLookupRepository;
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(QuickAddProductOptionCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<BaseResponseDto> serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    try
    {
      var product = await _productRepository.GetProductByIdAsync(new ProductId(new Guid(request.ProductStock?.ProductId!)), _currentUser.ClientId!);

      if (product is null)
      {
        throw new EntityNotFoundException("Product", request.ProductStock?.ProductId!);
      }
      var productId = new ProductId(new Guid(request.ProductStock?.ProductId!));

      //remove existing option from list 
      foreach (var item in request.ProductStock!.ProductOptions!.ToList())
      {
        var chkUnique = await CheckUniqueOptionNameByOptionIdNameAndProductId(productId, item.OptionId!, item.OptionValue);
        if (chkUnique)
        {
          request?.ProductStock?.ProductOptions!.Remove(item);
        }
      }

      //

      var pslist = new List<ProductStockReuqestModel>();

      var stationQuantities = request?.ProductStock?.StationQuantities;
      if (stationQuantities == null || !stationQuantities.Any())
      {
          // Fallback if StationQuantities is not provided
          stationQuantities = new List<StationQuantityRequest>();
          var stationIdsToCreate = request?.ProductStock?.ProductStationIds;
          if (stationIdsToCreate == null || !stationIdsToCreate.Any())
          {
              var allStations = await _stationLookupRepository.GetAllStationLookup(_currentUser.ClientId!);
              stationIdsToCreate = allStations?.Select(x => x.ProductStationId).ToList() ?? new List<int>();
          }
          foreach(var sid in stationIdsToCreate)
          {
              stationQuantities.Add(new StationQuantityRequest { StationId = sid, Quantity = request!.ProductStock!.QuantityAvailable.GetValueOrDefault() });
          }
      }

      foreach (var sq in stationQuantities)
      {
        var obj = new ProductStockReuqestModel()
        {
          ProductStockId = request!.ProductStock!.ProductStockId,
          ProductId = request.ProductStock!.ProductId,
          Sku = request.ProductStock!.Sku,
          Price = request.ProductStock!.Price,
          QuantityAvailable = sq.Quantity,
          ProductStationId = sq.StationId,
          VarientOption = request.ProductStock!.VarientOption,
          ProductStockStatusId = request.ProductStock!.ProductStockStatusId,
          Active = true
        };
        pslist.Add(obj);
      }
      var productCommon = new ProductCommon();
       
      var productStocks = productCommon.GetProductStocks(pslist, productId);
       
      var productOptions = productCommon.GetProductOptions(request!.ProductStock!.ProductOptions, productId);
      dynamic result = await _productRepository.CreateQuickAddProductOption(productOptions, productStocks);
      
      // CREATE ProductVariant and InventoryBalance
      var variant = ProductVariant.Create(
          productId, 
          _currentUser.ClientId, 
          request.ProductStock!.Sku, 
          request.ProductStock!.VariantAttributes?.Barcode, 
          request.ProductStock!.Price, 
          product.PurchasePrice, 
          request.ProductStock!.VariantAttributes?.Weight ?? product.Weight, 
          request.ProductStock!.VariantAttributes?.Length, 
          request.ProductStock!.VariantAttributes?.Width, 
          request.ProductStock!.VariantAttributes?.Height, 
          request.ProductStock!.LowQuantityLimit, 
          request.ProductStock!.VarientOption,
          request.ProductStock!.ImageGalleryId, 
          request.ProductStock!.ProductStockStatusId ?? 1, 
          _currentUser.EmployeeId
      );
      await _productRepository.CreateProductVariantAsync(variant);

      var balances = new List<InventoryBalance>();
      var transactions = new List<InventoryTransaction>();

      foreach (var sq in stationQuantities)
      {
          var balance = InventoryBalance.Create(variant.ProductVariantId, sq.StationId, sq.Quantity);
          balances.Add(balance);

          if (sq.Quantity > 0 && product.TrackInventory.GetValueOrDefault())
          {
              var transaction = InventoryTransaction.Create(
                  variant.ProductVariantId,
                  sq.StationId,
                  (int)InventoryTransactionType.Receipt,
                  sq.Quantity,
                  0,
                  sq.Quantity,
                  "New Variant Created with stock",
                  _currentUser.EmployeeId
              );
              transactions.Add(transaction);
          }
      }

      if (balances.Any())
      {
          await _productRepository.CreateInventoryBalancesAsync(balances);
      }

      if (transactions.Any())
      {
          await _productRepository.CreateInventoryTransactionsAsync(transactions);
      }

      //update product stock variant count
      product.UpdateVarientCount(result.ProductStocks.Count,_currentUser.EmployeeId!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<bool> CheckUniqueOptionNameByOptionIdNameAndProductId(ProductId productId, string? optionId, string? optionValue)
  {
    bool isExist = false;
    var target = await _productRepository.CheckUniqueOptionNameByOptionIdNameAndProductId(optionId, productId,optionValue);

    if (target == null)
    {
      isExist = false;  
    }
    else
    {
      isExist = true; 
    }
    return isExist;
  }
}
public class QuickAddProductOptionCommandValidator : AbstractValidator<QuickAddProductOptionCommand>
{
  public QuickAddProductOptionCommandValidator()
  {
    RuleForEach(model => model.ProductStock!.ProductOptions).SetValidator(model => new CreateProductOptionsValidator());
    // RuleFor(v => v.ProductStock!.ProductStationId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.ProductStock!.Sku).NotNull().NotEmpty();
    RuleFor(v => v.ProductStock!.VarientOption).NotNull().NotEmpty();
    RuleFor(v => v.ProductStock!.QuantityAvailable).NotNull().NotEmpty();//.GreaterThan(0);
  }
}

