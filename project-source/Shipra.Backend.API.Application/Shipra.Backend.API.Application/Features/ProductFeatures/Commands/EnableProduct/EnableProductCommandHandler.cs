using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.EnableProduct;
public class EnableProductCommandHandler : RequestHandlerBase<EnableProductCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  public EnableProductCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<EnableProductCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(EnableProductCommand request, CancellationToken cancellationToken)
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
      product.EnableProduct(_currentUser.EmployeeId!);
      var isUpdate = await _productRepository.UpdateProductAsync(product);

      var productStockList = await _productRepository.GetProductStockByProductIdAsync(productId);
      foreach (var oProductStock in productStockList!)
      {
        oProductStock!.EnableProductStockStaus(_currentUser.EmployeeId!);
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
