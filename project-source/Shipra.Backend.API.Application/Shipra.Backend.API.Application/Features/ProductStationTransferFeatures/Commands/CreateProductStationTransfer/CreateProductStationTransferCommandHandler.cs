using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;
using Shipra.Backend.API.Application.Features.ProductFeatures.Commands;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Commands.CreateProductStationTransfer;
public class CreateProductStationTransferCommandHandler : RequestHandlerBase<CreateProductStationTransferCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IProductStationTransferRepository _productStationTransferRepository;

  public CreateProductStationTransferCommandHandler(IProductStationTransferRepository productStationTransferRepository, IServiceProvider serviceProvider, ILogger<CreateProductStationTransferCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productStationTransferRepository = productStationTransferRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(CreateProductStationTransferCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    try
    {
      var productCommon = new ProductCommon();
      var productStationTransfer = GetProductStationsTransfer(request);
      var transferProductList = productCommon.GetTransferProducts(request.TransferProducts, productStationTransfer.ProductStaionTransferId);
      var result = await _productStationTransferRepository.CreateProductStationTransfer(productStationTransfer, transferProductList);

      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;

    }
  }

  private List<TransferProduct> GetTransferProducts(List<TransferProductRequestModel>? transferProducts, ProductStaionTransferId? productStaionTransferId)
  {
    var transferProductList = new List<TransferProduct>();
    foreach (var item in transferProducts!)
    {
      var transferProduct = TransferProduct.CreateTransferProduct(productStaionTransferId!, item.ProductSku, new ProductId(new Guid(item.ProductId!)), item.Quantity, item.Accepted, item.Rejected);
      transferProductList.Add(transferProduct);
    }
    return transferProductList;
  }

  private ProductStationTransfer GetProductStationsTransfer(CreateProductStationTransferCommand? request)
  {
    var productStationTransfer = ProductStationTransfer.CreateStationTransfer(request?.TrackingNo, request?.OriginProductStationId, request?.DestinationProductStationId, request?.ExpectedArrivalTime, request?.TransferStatusId, _currentUser.EmployeeId);
    return productStationTransfer;
  }

}

