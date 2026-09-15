using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.GetCarrierSettlementSamplefile;
public class GetCarrierSettlementSamplefileQuery : IRequest<ServiceResultDTO>
{
}
public class GetCarrierSettlementSamplefileQueryHandler : RequestHandlerBase<GetCarrierSettlementSamplefileQuery, ServiceResultDTO>
{
  public GetCarrierSettlementSamplefileQueryHandler(IServiceProvider serviceProvider, ILogger<GetCarrierSettlementSamplefileQueryHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCarrierSettlementSamplefileQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      await Task.Delay(1); 
      serviceResult = new ServiceResultDTO(new { url = ApplicationConstants.CarrierSettlementFileUrl });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
