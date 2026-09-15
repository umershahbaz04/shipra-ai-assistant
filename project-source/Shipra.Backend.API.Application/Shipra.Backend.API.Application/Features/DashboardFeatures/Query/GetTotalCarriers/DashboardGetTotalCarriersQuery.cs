using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.DashboardUserCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetTotalCarriers;
public class DashboardGetTotalCarriersQuery : IRequest<ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>>
{
  public FilterDateClientModel? FilterModel { get; set; }
}
public class GetTotalCarriersQueryHandler : RequestHandlerBase<DashboardGetTotalCarriersQuery, ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetTotalCarriersQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetTotalCarriersQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>> HandleRequest(DashboardGetTotalCarriersQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var data = await _carrierRepository.DashbaordGetAllCarriers(filter.CreatedFrom, filter.CreatedTo);
      DashboardDataWithCountResponseModel responseModel = new DashboardDataWithCountResponseModel();

      responseModel.Count = data.Count;
      responseModel.list = data;
      serviceResult = new ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>(responseModel);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
