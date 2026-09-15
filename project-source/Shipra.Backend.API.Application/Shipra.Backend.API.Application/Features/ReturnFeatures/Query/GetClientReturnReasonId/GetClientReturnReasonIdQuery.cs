using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetClientReturnReasonId;
public class GetClientReturnReasonIdQuery : IRequest<ServiceResultDTO>
{
  public int ClientReturnReasonId { get; set; }
}
public class GetClientReturnReasonIdQueryHandler : RequestHandlerBase<GetClientReturnReasonIdQuery, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public GetClientReturnReasonIdQueryHandler(IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<GetClientReturnReasonIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientReturnReasonIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var obj = await _returnRepository.GetClientReturnResonByClientReasonId(request.ClientReturnReasonId, _currentUser.ClientId!);
      if (obj is null)
      {
        throw new EntityNotFoundException("ClientReturnReson ", request.ClientReturnReasonId!);
      }
      var newObj = new
      {
        obj.ClientReturnReasonId,
        obj.Reason,
        obj.ReasonDetail
      };
      serviceResult = new ServiceResultDTO(newObj); 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
