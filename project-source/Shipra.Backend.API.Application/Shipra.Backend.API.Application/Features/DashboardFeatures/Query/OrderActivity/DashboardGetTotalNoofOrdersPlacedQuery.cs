using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.DashboardUserCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.OrderActivity;
public class DashboardGetTotalNoofOrdersPlacedQuery : IRequest<ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>>
{
  public FilterDateClientModel? FilterModel { get; set; }
}
public class DashboardGetTotalNoofOrdersPlacedQueryHandler : RequestHandlerBase<DashboardGetTotalNoofOrdersPlacedQuery, ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>>
{
  private readonly IOrderRepository _orderRepository;

  public DashboardGetTotalNoofOrdersPlacedQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<DashboardGetTotalNoofOrdersPlacedQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>> HandleRequest(DashboardGetTotalNoofOrdersPlacedQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>();
    try
    {
      var filter = request.FilterModel!;
      var data = await _orderRepository.DashboardGetTotalNoofOrdersPlaced(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
      var responseModel = new DashboardDataWithCountResponseModel();

      responseModel.Count = data.TotalCount;
      //responseModel.list = data;
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
