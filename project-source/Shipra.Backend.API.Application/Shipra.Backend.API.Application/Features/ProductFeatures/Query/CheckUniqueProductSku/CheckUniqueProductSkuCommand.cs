using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.CheckUniqueProductSku;
public class CheckUniqueProductSkuQuery : IRequest<ServiceResultDTOWithTypeModel<ExistProductResponseModel>>
{
  public string? Sku { get; set; } 
}
public class CheckUniqueProductSkuQueryHandler : RequestHandlerBase<CheckUniqueProductSkuQuery, ServiceResultDTOWithTypeModel<ExistProductResponseModel>>
{
  private readonly IProductRepository _productRepository;

  public CheckUniqueProductSkuQueryHandler(IProductRepository productRepository,IServiceProvider serviceProvider, ILogger<CheckUniqueProductSkuQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExistProductResponseModel>> HandleRequest(CheckUniqueProductSkuQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<ExistProductResponseModel>();
    try
    {
      var target = await _productRepository.CheckUniqueProductSku(request.Sku!, _currentUser.ClientId!);
      ExistProductResponseModel response = new ExistProductResponseModel();
      if (target != null)
      {
        response.IsExist = true;
        response.Message = $"Product with SKU :{request.Sku} already exist";
        serviceResult.CreateSuccessResponse();
      }
      else
      {
        response.Message = "Sku is available";
      }
      serviceResult = new ServiceResultDTOWithTypeModel<ExistProductResponseModel>(response);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
