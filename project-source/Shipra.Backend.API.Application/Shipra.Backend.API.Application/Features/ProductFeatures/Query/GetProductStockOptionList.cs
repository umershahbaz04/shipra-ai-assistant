using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query;
public class GetProductStockOptionList : IRequest<ServiceResultDTOWithTypeModel<ProductResponseModel>>
{
  public string? ProductId { get; set; }
}
public class GetProductStockOptionListHandler : RequestHandlerBase<GetProductStockOptionList, ServiceResultDTOWithTypeModel<ProductResponseModel>>
{
  private readonly IProductRepository _productRepository;
  public GetProductStockOptionListHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ProductResponseModel>> HandleRequest(GetProductStockOptionList request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ProductResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ProductResponseModel>();
    try
    {
      Guid guidID;
      var hasGUID = Guid.TryParse(request!.ProductId!, out guidID);
      if (!hasGUID)
      {
        throw new InvalidIdTypeException(request!.ProductId!);
      }
      var productId = new ProductId(new Guid(request!.ProductId!));

      var product = await _productRepository.GetProductByIdAsync(productId, _currentUser.ClientId!);
      if (product is null)
      {
        throw new EntityNotFoundException("Product", productId.Value);
      }

      var productDTO = _mapper.Map<ProductResponseModel>(product);

      var productOptions = await _productRepository.GetProductOptionByProductIdAsync(productId);
      var productStocks = await _productRepository.GetProductStockByProductIdAsync(productId);

      var productOptionMapping = _mapper.Map<List<ProductOptionReuqestModel>>(productOptions);
      var productStockMapping = _mapper.Map<List<ProductStockReuqestModel>>(productStocks);

      productDTO.ProductOptions = productOptionMapping;
      productDTO.ProductStocks = productStockMapping;
      serviceResult = new ServiceResultDTOWithTypeModel<ProductResponseModel>(productDTO);

      serviceResult.CreateSuccessResponse();
      return serviceResult;

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
