using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetSMSActivateBySMSLookupId;
public class GetSMSActivateBySMSLookupIdQuery : IRequest<ServiceResultDTO>
{
}
public class GetSMSActivateBySMSLookupIdQueryHandler : RequestHandlerBase<GetSMSActivateBySMSLookupIdQuery, ServiceResultDTO>
{
  public GetSMSActivateBySMSLookupIdQueryHandler(IServiceProvider serviceProvider, ILogger<GetSMSActivateBySMSLookupIdQueryHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSMSActivateBySMSLookupIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      await Task.Delay(1);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
