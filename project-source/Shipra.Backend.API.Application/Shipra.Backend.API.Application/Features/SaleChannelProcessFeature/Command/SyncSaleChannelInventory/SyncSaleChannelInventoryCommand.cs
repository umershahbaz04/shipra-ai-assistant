using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SyncSaleChannelInventory;
public class SyncSaleChannelInventoryCommand : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public int SaleChannelLookupId { get; set; }
  public int SaleChannelConfigId { get; set; }
  public List<SyncInventoryModel>? List { get; set; } = new();
} 
public class SyncSaleChannelInventoryCommandHandler : RequestHandlerBase<SyncSaleChannelInventoryCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public SyncSaleChannelInventoryCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<SyncSaleChannelInventoryCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SyncSaleChannelInventoryCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    { 
      List<SyncInventoryResponseModel> syncResults = new();
      foreach (var pStock in request.List!)
      {
        var syncResponse = new SyncInventoryResponseModel
        {
          ProductStockId = pStock.ProductStockId,
          Sku = pStock.Sku,
          Quantity = pStock.Quantity,
          ProdStationId = pStock.ProdStationId
        };

        try
        {
          var oProductVariant = await _productRepository.GetProductVariantBySKUAsync(pStock.Sku!);
          if (oProductVariant is null)
          {
             syncResponse.IsSuccess = false;
             syncResponse.Error = "Product variant not found.";
          }
          else
          {
              var inventoryBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(oProductVariant.ProductVariantId, pStock.ProdStationId);
              if (inventoryBalance is null)
              {
                 syncResponse.IsSuccess = false;
                 syncResponse.Error = "Inventory balance record not found.";
              }
              else if (pStock.Quantity != inventoryBalance.QuantityAvailable)
              {
                 int previousQty = inventoryBalance.QuantityAvailable;
                 inventoryBalance.UpdateQuantities(pStock.Quantity, pStock.Quantity, inventoryBalance.QuantityCommitted);
                 var updated = await _productRepository.UpdateInventoryBalanceAsync(inventoryBalance);
                 if (updated is not null)
                 {
                   string? saleChannel = EnumHelper.GetFormattedEnumName<EnumSaleChannelLookup>(request.SaleChannelLookupId);
                   var transaction = InventoryTransaction.Create(
                       inventoryBalance.ProductVariantId,
                       inventoryBalance.ProductStationId,
                       (int)InventoryTransactionType.ExternalSync,
                       Math.Abs(previousQty - pStock.Quantity),
                       previousQty,
                       pStock.Quantity,
                       $"{saleChannel} Inventory Sync",
                       _currentUser.EmployeeId!
                   );
                   await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> { transaction });
                   
                   syncResponse.IsSuccess = true;
                   syncResponse.Error = string.Empty;
                 }
                 else
                 {
                   syncResponse.IsSuccess = false;
                   syncResponse.Error = "Inventory update failed.";
                 }
              }
              else
              {
                 syncResponse.IsSuccess = true;
                 syncResponse.Error = string.Empty;
              }
          }
        }
        catch (Exception ex)
        {
          syncResponse.IsSuccess = false;
          syncResponse.Error = ex.Message;
        }

        syncResults.Add(syncResponse);
      }

      serviceResult = new ServiceResultDTO(new BaseResponseDto { });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  } 
}
public class SyncSaleChannelInventoryCommandValidator : AbstractValidator<SyncSaleChannelInventoryCommand>
{
  //public int StoreId { get; set; }
  //public int SaleChannelLookupId { get; set; }
  //public int SaleChannelConfigId { get; set; }
  //public List<SyncInventoryModel>? List { get; set; } = new();
  public SyncSaleChannelInventoryCommandValidator()
  {
    RuleFor(x => x.SaleChannelLookupId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.SaleChannelConfigId).NotEmpty().NotNull().GreaterThan(0);

    RuleFor(x => x.List).Must(x => x != null).WithMessage("list must contain at least one item.");

    RuleForEach(x => x.List).SetValidator(x => new SyncInventoryModelValidator()); 
  }
}
public class SyncInventoryModelValidator : AbstractValidator<SyncInventoryModel>
{
  //public int StoreId { get; set; }
  //public int SaleChannelLookupId { get; set; }
  //public int SaleChannelConfigId { get; set; }
  //public List<SyncInventoryModel>? List { get; set; } = new();
  public SyncInventoryModelValidator()
  {
    RuleFor(v => v.Sku).NotNull().NotEmpty(); 
    RuleFor(v => v.ProductStockId).NotNull().NotEmpty().GreaterThan(0); 
  }
}
