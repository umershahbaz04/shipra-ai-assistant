using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.CheckUniqueProductStockSku;
public class CheckUniqueProductStockSkuQuery : IRequest<ServiceResultDTOWithTypeModel<ExistProductResponseModel>>
{
  public string? Sku { get; set; }
}
public class CheckUniqueProductStockSkuHandler : RequestHandlerBase<CheckUniqueProductStockSkuQuery, ServiceResultDTOWithTypeModel<ExistProductResponseModel>>
{
  private readonly IProductRepository _productRepository;
  public CheckUniqueProductStockSkuHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<CheckUniqueProductStockSkuHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ExistProductResponseModel>> HandleRequest(CheckUniqueProductStockSkuQuery request, CancellationToken cancellationToken)
  {

    var serviceResult = new ServiceResultDTOWithTypeModel<ExistProductResponseModel>();
    try
    {
      var target = await _productRepository.CheckUniqueProductStockSKU(request.Sku!, _currentUser.ClientIdStr!);
      var response = new ExistProductResponseModel();
      if (target)
      {
        response.IsExist = true;
        response.Message = $"Product Stock with SKU :{request.Sku} already exist";
        serviceResult.CreateSuccessResponse();
      }
      else
      {
        response.Message = "Sku is unavailable";
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
public class CheckUniqueProductStockSkuQueryValidator : AbstractValidator<CheckUniqueProductStockSkuQuery>
{
  public CheckUniqueProductStockSkuQueryValidator()
  {
    RuleFor(x => x.Sku).NotNull().NotEmpty();
  }
}
