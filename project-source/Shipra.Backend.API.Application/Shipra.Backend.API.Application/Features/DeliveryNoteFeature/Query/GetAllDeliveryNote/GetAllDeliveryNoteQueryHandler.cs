using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetAllDeliveryNote;
public class GetAllDeliveryNoteQueryHandler : RequestHandlerBase<GetAllDeliveryNoteQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public GetAllDeliveryNoteQueryHandler(IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<GetAllDeliveryNoteQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDeliveryNoteQuery request, CancellationToken cancellationToken)
  {
    var serviceResultDTO = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      dynamic deliveryNote = await _deliveryNoteRepository.GetAllDeliveryNote(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, request!.DeliveryNoteStatusId, _currentUser.ClientIdStr, request.DriverIds);
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

