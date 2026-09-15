using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.DisableProduct;
public class DisableProductCommandHandler : RequestHandlerBase<DisableProductCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  public DisableProductCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<DisableProductCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(DisableProductCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var productId = new ProductId(new Guid(request!.ProductId!));

      var product = await _productRepository.GetProductByIdAsync(productId, _currentUser.ClientId!);

      if (product is null)
      {
        throw new EntityNotFoundException("Product", productId.Value);
      }
      product.DisableProduct(_currentUser.EmployeeId!);
      var isUpdate = await _productRepository.UpdateProductAsync(product);

      var productStockList = await _productRepository.GetProductStockByProductIdAsync(productId);
      foreach (var oProductStock in productStockList!)
      { 
        oProductStock!.DisableProductStockStaus(_currentUser.EmployeeId!);
        isUpdate = await _productRepository.UpdateProductStockAsync(oProductStock);
      }

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
