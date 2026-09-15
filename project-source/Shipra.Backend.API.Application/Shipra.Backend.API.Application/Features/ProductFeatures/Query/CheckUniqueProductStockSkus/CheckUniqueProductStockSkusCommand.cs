using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.CheckUniqueProductStockSkus;
public class CheckUniqueProductStockSkusCommand : IRequest<ServiceResultDTO>
{
  public string? Skus { get; set; }
}
public class CheckUniqueProductStockSkusCommandHandler : RequestHandlerBase<CheckUniqueProductStockSkusCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public CheckUniqueProductStockSkusCommandHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<CheckUniqueProductStockSkusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CheckUniqueProductStockSkusCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var listSkus = request.Skus?.Split(',').ToList();
      var errorList = new List<ExistProductResponseModel>();
      foreach (var Sku in listSkus!)
      {
        var target = await _productRepository.CheckUniqueProductStockSKU(Sku, _currentUser.ClientIdStr!);
        ExistProductResponseModel response = new ExistProductResponseModel();
        if (target)
        {
          response.IsExist = true;
          response.Message = $"Product Variant with SKU :{Sku} already exist";
          response.Sku = Sku; 
          errorList.Add(response);
        } 
      }
      int count = 1;
      if (errorList.Count > 0)
      {
        foreach (var error in errorList)
        {
          //may be duplicate sku
          serviceResult.CreateError(error!.Sku!+ count, new string[] { error?.Message! });
          count++;
        }
      } 
      return serviceResult;
    }
    catch (Exception)
    {

      throw;
    }
  }
}
public class CheckUniqueProductStockSkusCommandValidator : AbstractValidator<CheckUniqueProductStockSkusCommand>
{
  public CheckUniqueProductStockSkusCommandValidator()
  {
    RuleFor(x => x.Skus).NotEmpty().NotNull();
  }
}
