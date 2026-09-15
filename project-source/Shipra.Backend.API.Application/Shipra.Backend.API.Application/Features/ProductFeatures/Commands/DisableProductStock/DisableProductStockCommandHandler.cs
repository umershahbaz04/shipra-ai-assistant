using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.DisableProductStock;
public class DisableProductStockCommandHandler : RequestHandlerBase<DisableProductStockCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  public DisableProductStockCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<DisableProductStockCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(DisableProductStockCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var oProductStock = await _productRepository.GetProductStockByIdAsync(request.ProductStockId);
      if (oProductStock is null)
      {
        throw new EntityNotFoundException("Product Stock", request!.ProductStockId!);
      }
      oProductStock!.DisableProductStockStaus(_currentUser.EmployeeId!);
      var isUpdate = await _productRepository.UpdateProductStockAsync(oProductStock);
      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Message = NotificationConstants.UpdateSuccess
      });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
