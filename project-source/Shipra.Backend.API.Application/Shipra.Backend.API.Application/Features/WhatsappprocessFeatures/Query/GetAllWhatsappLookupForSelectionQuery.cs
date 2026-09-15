using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSLookupForSelection;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SMSProcessAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Query;
public class GetAllWhatsappLookupForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllWhatsappLookupForSelectionQueryHandler : RequestHandlerBase<GetAllWhatsappLookupForSelectionQuery, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;

  public GetAllWhatsappLookupForSelectionQueryHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<GetAllWhatsappLookupForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllWhatsappLookupForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _smsProcessRepository.GetAllWhatsappLookupForSelection();
      var obj = WhatsappLookup.AddDefault();
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
