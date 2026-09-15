using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DeliveryNoteUseCase.Response;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetDeliveryTaskById;
public class GetDeliveryTaskByIdQueryHandler : RequestHandlerBase<GetDeliveryTaskByIdQuery, ServiceResultDTO>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public GetDeliveryTaskByIdQueryHandler(IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<GetDeliveryTaskByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDeliveryTaskByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    try
    {
      var deliveryTaskId = new DeliveryTaskId(new Guid(request.DeliveryTaskId!));
      var deliveryTask = await _deliveryTaskRepository.GetDeliveryTaskById(deliveryTaskId);
      if (deliveryTask is not null)
      {
        var model = _mapper.Map<DeliveryTaskResponseModel>(deliveryTask);
        serviceResultDTO = new ServiceResultDTO(model);
        serviceResultDTO.CreateSuccessResponse(HttpStatusCode.OK);
      }
      else
      {
        throw new EntityNotFoundException("DeliveryNote ", deliveryTaskId!);
      }
      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}

