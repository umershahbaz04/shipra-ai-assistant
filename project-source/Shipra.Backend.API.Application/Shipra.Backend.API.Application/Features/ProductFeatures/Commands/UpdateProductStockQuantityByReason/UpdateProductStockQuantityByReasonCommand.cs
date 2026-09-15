using System.Dynamic;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UpdateProductStockQuantityByReason;
public class UpdateProductStockQuantityByReasonCommand : IRequest<ServiceResultDTO>
{
  public long ProductStockId { get; set; }
  public int TransactionTypeId { get; set; }
  public int Quantity { get; set; }
  public string? Comment { get; set; }
}
public class UpdateProductStockQuantityByReasonCommandHandler : RequestHandlerBase<UpdateProductStockQuantityByReasonCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public UpdateProductStockQuantityByReasonCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<UpdateProductStockQuantityByReasonCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateProductStockQuantityByReasonCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var stock = await _productRepository.GetInventoryBalanceByIdAsync(request.ProductStockId);
      string msg = string.Empty;

      if (stock is null)
      {
        throw new EntityNotFoundException("InventoryBalance", request.ProductStockId);
      }

      int oldDamage = stock.QuantityDamaged;
      int qty = stock.QuantityAvailable;

      int newQuantityDamage = oldDamage;
      int newQuantityAvailable = qty;

      if (request.TransactionTypeId == (int)InventoryTransactionType.Damage)
      {
          if (qty >= request.Quantity)
          {
            newQuantityDamage = oldDamage + request.Quantity;
            newQuantityAvailable = qty - request.Quantity;
          }
          else
          {
            msg = $"Stock quantity is less than added quantity";
          }
      }
      else if (request.TransactionTypeId == (int)InventoryTransactionType.Return) // Damage Replace
      {
          if (oldDamage >= request.Quantity)
          {
            newQuantityDamage = oldDamage - request.Quantity;
            newQuantityAvailable = qty + request.Quantity;
          }
          else
          {
            msg = $"Damage quantity is less than added quantity";
          }
      }
      else if (request.TransactionTypeId == (int)InventoryTransactionType.Receipt)
      {
          newQuantityAvailable = qty + request.Quantity;
      }
      else if (request.TransactionTypeId == (int)InventoryTransactionType.Adjustment)
      {
          newQuantityAvailable = request.Quantity;
      }
      
      if (string.IsNullOrEmpty(msg))
      {
          if (request.TransactionTypeId == (int)InventoryTransactionType.Damage || request.TransactionTypeId == (int)InventoryTransactionType.Return)
          {
              stock.UpdateDamagedQuantity(newQuantityDamage, newQuantityAvailable);
          }
          else
          {
              stock.UpdateQuantities(newQuantityAvailable, newQuantityAvailable, stock.QuantityCommitted);
          }
          
          var updatedStock = await _productRepository.UpdateInventoryBalanceAsync(stock);

          var stockHistory = InventoryTransaction.Create(
            stock.ProductVariantId, 
            stock.ProductStationId, 
            request.TransactionTypeId, 
            request.TransactionTypeId == (int)InventoryTransactionType.Adjustment ? Math.Abs(qty - request.Quantity) : request.Quantity, 
            qty, 
            newQuantityAvailable, 
            request.Comment!, 
            _currentUser.EmployeeId!
          );
          await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> { stockHistory });
      }
      else
      {
          serviceResult.IsSuccess = false;
          serviceResult.Errors?.Add("Error", new string[] { msg });
      }


      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

public class UpdateProductStockQuantityByReasonCommandValidator : AbstractValidator<UpdateProductStockQuantityByReasonCommand>
{
  public UpdateProductStockQuantityByReasonCommandValidator()
  {
    RuleFor(v => v.ProductStockId).NotEmpty().NotNull().GreaterThan(0);
    //RuleFor(v => v.Quantity).NotEmpty().NotNull().GreaterThan(0);
  }
}
