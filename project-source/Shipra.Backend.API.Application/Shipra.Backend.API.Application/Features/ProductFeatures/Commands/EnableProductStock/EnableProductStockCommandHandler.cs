using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.EnableProductStock;
internal class EnableProductStockCommandHandler : RequestHandlerBase<EnableProductStockCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  public EnableProductStockCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<EnableProductStockCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(EnableProductStockCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var oProductStock = await _productRepository.GetProductStockByIdAsync(request.ProductStockId);
      if (oProductStock is null)
      {
        throw new EntityNotFoundException("Product Stock", request.ProductStockId!);
      }
      oProductStock!.EnableProductStockStaus(_currentUser.EmployeeId!);
      var isUpdate = await _productRepository.UpdateProductStockAsync(oProductStock);
      #region if product disabled then update 
      var oProduct = await _productRepository.GetProductByIdAsync(oProductStock.ProductId!, _currentUser.ClientId);
      if (oProduct is not null)
      {
        if (!oProduct.Active.GetValueOrDefault())
        {
          oProduct.EnableProduct(_currentUser.EmployeeId!);
          await _productRepository.UpdateProductAsync(oProduct);
        }
      }
      #endregion
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
