using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Query.GetAllProductStationTransfer;
public class GetAllProductStationTransferQuery : IRequest<ServiceResultDTOWithTypeModel<List<ProductStationTransferResponseModel>>>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllProductStationTransferQueryHandler : RequestHandlerBase<GetAllProductStationTransferQuery, ServiceResultDTOWithTypeModel<List<ProductStationTransferResponseModel>>>
{
  private readonly IProductStationTransferRepository _stationTransferRepository;
  public GetAllProductStationTransferQueryHandler(IProductStationTransferRepository stationTransferRepository, IServiceProvider serviceProvider, ILogger<GetAllProductStationTransferQueryHandler> logger) : base(serviceProvider, logger)
  {
    _stationTransferRepository = stationTransferRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<ProductStationTransferResponseModel>>> HandleRequest(GetAllProductStationTransferQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<List<ProductStationTransferResponseModel>> serviceResult = new ServiceResultDTOWithTypeModel<List<ProductStationTransferResponseModel>>();

    try
    {
      var filter = request.FilterModel!;
      var data = await _stationTransferRepository.GetAllProductStationTransfer(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol!, filter.SortDir!);

      if (data is null)
      {
        throw new Exception("No ProductStationTransferExits ");
      }

      //  var transferProducts = await _stationTransferRepository.GetTransferProductByProductStationTransferId(data.ProductStaionTransferId!);


      //var transferProductMapping = _mapper.Map<List<TransferProductRequestModel>>(transferProducts);
      var responseDto = _mapper.Map<List<ProductStationTransferResponseModel>>(data);

      serviceResult = new ServiceResultDTOWithTypeModel<List<ProductStationTransferResponseModel>>(responseDto);
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
