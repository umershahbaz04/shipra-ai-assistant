using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Query.GetProductStationTransferById;
public class GetProductStationTransferByIdQuery : IRequest<ServiceResultDTOWithTypeModel<ProductStationTransferResponseModel>>
{
  public string? ProductStationTransferId { get; set; }
}
public class GetProductStationTransferByIdQueryHandler : RequestHandlerBase<GetProductStationTransferByIdQuery, ServiceResultDTOWithTypeModel<ProductStationTransferResponseModel>>
{
  private readonly IProductStationTransferRepository _productStationTransferRepository;
  public GetProductStationTransferByIdQueryHandler(IProductStationTransferRepository productStationTransferRepository, IServiceProvider serviceProvider, ILogger<GetProductStationTransferByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productStationTransferRepository = productStationTransferRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ProductStationTransferResponseModel>> HandleRequest(GetProductStationTransferByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ProductStationTransferResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ProductStationTransferResponseModel>();
    try
    {
      Guid guidID;
      var hasGUID = Guid.TryParse(request!.ProductStationTransferId!, out guidID);
      if (!hasGUID)
      {
        throw new InvalidIdTypeException(request!.ProductStationTransferId!);
      }
      var productStationTranferId = new ProductStaionTransferId(new Guid(request!.ProductStationTransferId!));
      var productStationTransfer = await _productStationTransferRepository.GetProductStationTransferById(productStationTranferId);
      if (productStationTransfer == null)
      {
        throw new EntityNotFoundException("Product Station Transfer", productStationTranferId.Value);
      }
      var productStationTransferDTO = _mapper.Map<ProductStationTransferResponseModel>(productStationTransfer);

      var transferProducts = await _productStationTransferRepository.GetTransferProductByProductStationTransferId(productStationTranferId);
      var transferProductMapping = _mapper.Map<List<TransferProductRequestModel>>(transferProducts);
      productStationTransferDTO.TransferProducts = transferProductMapping;
      serviceResult = new ServiceResultDTOWithTypeModel<ProductStationTransferResponseModel>(productStationTransferDTO);

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
