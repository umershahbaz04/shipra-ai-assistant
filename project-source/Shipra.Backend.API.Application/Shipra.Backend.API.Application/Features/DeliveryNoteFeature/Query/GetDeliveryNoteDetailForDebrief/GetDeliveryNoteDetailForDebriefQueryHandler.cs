using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteDetailForDebrief;
public class GetDeliveryNoteDetailForDebriefQueryHandler : RequestHandlerBase<GetDeliveryNoteDetailForDebriefQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  public GetDeliveryNoteDetailForDebriefQueryHandler(IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<GetDeliveryNoteDetailForDebriefQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetDeliveryNoteDetailForDebriefQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var deliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailForDebrief(request!.DeliveryNoteId!,_currentUser.ClientIdStr!);

      serviceResult = new ServiceResultDTO(deliveryNoteDetail);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
