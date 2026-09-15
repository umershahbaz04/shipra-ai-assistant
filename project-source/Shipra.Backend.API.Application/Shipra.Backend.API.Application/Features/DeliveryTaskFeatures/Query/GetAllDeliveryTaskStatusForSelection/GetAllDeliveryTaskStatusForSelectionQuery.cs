using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllDeliveryTaskStatusForSelection;
public class GetAllDeliveryTaskStatusForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllDeliveryTaskStatusForSelectionQueryHandler : RequestHandlerBase<GetAllDeliveryTaskStatusForSelectionQuery, ServiceResultDTO>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public GetAllDeliveryTaskStatusForSelectionQueryHandler(IDeliveryTaskRepository deliveryTaskRepository,IServiceProvider serviceProvider, ILogger<GetAllDeliveryTaskStatusForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDeliveryTaskStatusForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    { 
      List<DeliveryTaskStatusLookup> data = await _deliveryTaskRepository.GetAllDeliveryTaskStatusForSelection();
      data.Add(DeliveryTaskStatusLookup.AddDefault());
      var newList = data.OrderBy(x => x.DeliveryTaskStatusId).ToList();
      serviceResult = new ServiceResultDTO(newList);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
