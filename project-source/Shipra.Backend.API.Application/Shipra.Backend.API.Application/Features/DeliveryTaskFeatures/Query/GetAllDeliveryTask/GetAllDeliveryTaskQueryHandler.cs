using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllDeliveryTask;
public class GetAllDeliveryTaskQueryHandler : RequestHandlerBase<GetAllDeliveryTaskQuery, ServiceResultDTO>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public GetAllDeliveryTaskQueryHandler(IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<GetAllDeliveryTaskQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDeliveryTaskQuery request, CancellationToken cancellationToken)
  {
    var serviceResultDTO = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      dynamic deliveryNote = await _deliveryTaskRepository.GetAllDeliveryTask(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, _currentUser.ClientId!.Value.ToString(), request?.DriverAssignedStatus!,request!.CountryId,request.CarrierTrackingStatusIds,request.StoreIds,request.DeliveryTaskStatusIds,request.DriverIds,request.IncludeDriver,request.OrderAddressFilter,request.SalePersonIds, request.OrderLabels, request.DuplicateStatus, request.DeliveryNoteStatusId);
      if (deliveryNote is not null)
      {
        serviceResultDTO = new ServiceResultDTO(deliveryNote);
        serviceResultDTO.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        return serviceResultDTO;
      }
      else
      {
        serviceResultDTO.CreateErrorResponse(new Exception(NotificationConstants.Error));
        return serviceResultDTO;
      }
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
