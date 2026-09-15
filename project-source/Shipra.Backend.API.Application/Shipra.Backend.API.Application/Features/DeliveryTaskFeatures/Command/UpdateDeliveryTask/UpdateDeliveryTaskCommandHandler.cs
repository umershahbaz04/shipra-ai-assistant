using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.UpdateDeliveryTask;
public class UpdateDeliveryTaskCommandHandler : RequestHandlerBase<UpdateDeliveryTaskCommand, ServiceResultDTO>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public UpdateDeliveryTaskCommandHandler(IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<UpdateDeliveryTaskCommandHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateDeliveryTaskCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    try
    {
      Guid guidID;
      var hasGUID = Guid.TryParse(request!.DeliveryTaskId!, out guidID);
      if (!hasGUID)
      {
        throw new InvalidIdTypeException(request!.DeliveryTaskId!);
      }
      var deliveryTaskId = new DeliveryTaskId(new Guid(request!.DeliveryTaskId!));

      var deliveryTask = await _deliveryTaskRepository.GetDeliveryTaskById(deliveryTaskId);
      if (deliveryTask is null)
      {
        throw new EntityNotFoundException("DeliveryTask", deliveryTaskId);
      }
      deliveryTask.UpdateDeliveryTask(request!.JobCode!, request?.DriverId!, request?.DriverPaid!, request?.DeliveryTaskStatusId!, request?.DriverReceivableId, request?.LastStatusUpdateId, request?.Active, request?.SortOrder);
      var oDeliveryTask = await _deliveryTaskRepository.UpdateDeliveryTask(deliveryTask);
      serviceResultDTO = new ServiceResultDTO(oDeliveryTask!);
      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
