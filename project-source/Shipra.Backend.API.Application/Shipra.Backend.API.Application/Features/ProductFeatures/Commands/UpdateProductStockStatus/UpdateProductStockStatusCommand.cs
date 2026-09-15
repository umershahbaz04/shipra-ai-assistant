using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UpdateProductStockStatus;
public class UpdateProductStockStatusCommand : IRequest<ServiceResultDTO>
{
  public int ProductStockId { get; set; }
  public int productStockStatusId { get; set; }
}
public class UpdateProductStockStatusCommandHandler : RequestHandlerBase<UpdateProductStockStatusCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public UpdateProductStockStatusCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<UpdateProductStockStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateProductStockStatusCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var stock = await _productRepository.GetProductStockByIdAsync(request.ProductStockId);

      if (stock is null)
      {
        throw new EntityNotFoundException("ProductStock", request.ProductStockId);
      }
      stock.UpdateProductStockStaus(request.productStockStatusId, _currentUser.EmployeeId!);
      var response = await _productRepository.UpdateProductStockAsync(stock);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
