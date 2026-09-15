using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SMSProcessAggregate;

namespace Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSLookupForSelection;
public class GetAllSMSLookupForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllSMSLookupForSelectionQueryHandler : RequestHandlerBase<GetAllSMSLookupForSelectionQuery, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;

  public GetAllSMSLookupForSelectionQueryHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<GetAllSMSLookupForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSMSLookupForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _smsProcessRepository.GetAllSMSLookupForSelection();
      var obj = SMSLookup.AddDefault();
      data?.Insert(0, obj);

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
