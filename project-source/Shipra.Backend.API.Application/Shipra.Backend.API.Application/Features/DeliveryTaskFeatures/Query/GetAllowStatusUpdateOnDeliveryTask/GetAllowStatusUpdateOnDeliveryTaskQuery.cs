using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllowStatusUpdateOnDeliveryTask;
public class GetAllowStatusUpdateOnDeliveryTaskQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllowStatusUpdateOnDeliveryTaskQueryHandler : RequestHandlerBase<GetAllowStatusUpdateOnDeliveryTaskQuery, ServiceResultDTO>
{
  public GetAllowStatusUpdateOnDeliveryTaskQueryHandler(IServiceProvider serviceProvider, ILogger<GetAllowStatusUpdateOnDeliveryTaskQueryHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllowStatusUpdateOnDeliveryTaskQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      await Task.Delay(1);

      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Data =
        new
        {
          Delivered = (int)EnumCarrierTrackingStatus.Delivered,
          Refunded = (int)EnumCarrierTrackingStatus.Refunded
        }
      });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
