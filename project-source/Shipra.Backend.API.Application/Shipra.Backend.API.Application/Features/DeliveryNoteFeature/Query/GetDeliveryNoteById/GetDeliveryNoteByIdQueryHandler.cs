using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DeliveryNoteUseCase.Response;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteById;
public class GetDeliveryNoteByIdQueryHandler : RequestHandlerBase<GetDeliveryNoteByIdQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  public GetDeliveryNoteByIdQueryHandler(IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<GetDeliveryNoteByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetDeliveryNoteByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var deliveryNoteId = new DeliveryNoteId(new Guid(request?.DeliveryNoteId!));
      var deliveryNote = await _deliveryNoteRepository.GetDeliveryNoteById(deliveryNoteId);
      if (deliveryNote is not null)
      {
        var data = _mapper.Map<DeliveryNoteResponseModel>(deliveryNote);
        serviceResult = new ServiceResultDTO(data);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      }
      else
      {
        throw new EntityNotFoundException("DeliveryNote ", deliveryNoteId!.Value);
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

