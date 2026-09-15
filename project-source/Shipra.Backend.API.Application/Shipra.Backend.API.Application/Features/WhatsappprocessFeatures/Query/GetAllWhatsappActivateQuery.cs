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
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSActivate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Query;
public class GetAllWhatsappActivateQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}

public class GetAllWhatsappActivateQueryHandler : RequestHandlerBase<GetAllWhatsappActivateQuery, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;

  public GetAllWhatsappActivateQueryHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<GetAllWhatsappActivateQueryHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllWhatsappActivateQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();

      var data = await _smsProcessRepository.GetAllWhatsappActivate(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId);

      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
