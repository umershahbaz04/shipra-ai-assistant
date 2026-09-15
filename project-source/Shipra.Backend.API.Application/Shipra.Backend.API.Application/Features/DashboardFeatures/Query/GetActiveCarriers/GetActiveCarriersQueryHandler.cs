using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DashboardUserCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetActiveCarriers;
public class GetActiveCarriersQueryHandler : RequestHandlerBase<GetActiveCarriersQuery, ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetActiveCarriersQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetActiveCarriersQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>> HandleRequest(GetActiveCarriersQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>();
    try
    {
      var data = await _carrierRepository.DashbaordGetAllActiveCarriers(_currentUser.ClientIdStr!);
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
