using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllPendingForReturnShipment;
public class GetAllPendingForReturnShipmentQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllPendingForReturnShipmentQueryHandler : RequestHandlerBase<GetAllPendingForReturnShipmentQuery, ServiceResultDTO>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public GetAllPendingForReturnShipmentQueryHandler(IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<GetAllPendingForReturnShipmentQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPendingForReturnShipmentQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      var data = await _deliveryTaskRepository.GetAllPendingForReturnShipments(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, _currentUser.ClientIdStr!);

      serviceResultDTO = new ServiceResultDTO(data);

      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
