using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SMSProcessAggregate;

namespace Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetSMSActivateById;
public class GetSMSActivateByIdQuery : IRequest<ServiceResultDTO>
{
  public int PpactivateId { get; set; }
}
public class GetSMSActivateByIdQueryHandler : RequestHandlerBase<GetSMSActivateByIdQuery, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;

  public GetSMSActivateByIdQueryHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<GetSMSActivateByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSMSActivateByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _smsProcessRepository.GetSMSActivateById(request.PpactivateId,_currentUser.ClientId!); 

      serviceResult = new ServiceResultDTO(data!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
