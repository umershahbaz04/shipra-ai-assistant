using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetSMSActivateById;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Query;
public class GetWhatsappActivateByIdQuery : IRequest<ServiceResultDTO>
{
  public int WhatsappactivateId { get; set; }
}
public class GetWhatsappActivateByIdQueryHandler : RequestHandlerBase<GetWhatsappActivateByIdQuery, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;

  public GetWhatsappActivateByIdQueryHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<GetWhatsappActivateByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetWhatsappActivateByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _smsProcessRepository.GetWhatsappActivateById(request.WhatsappactivateId, _currentUser.ClientId!);

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
