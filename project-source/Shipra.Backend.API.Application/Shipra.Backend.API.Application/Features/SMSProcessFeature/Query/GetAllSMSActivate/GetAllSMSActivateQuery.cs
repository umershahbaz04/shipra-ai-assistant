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

namespace Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSActivate;
public class GetAllSMSActivateQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllSMSActivateQueryHandler : RequestHandlerBase<GetAllSMSActivateQuery, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;

  public GetAllSMSActivateQueryHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<GetAllSMSActivateQueryHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSMSActivateQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();

      var data = await _smsProcessRepository.GetAllSMSActivate(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId);

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
