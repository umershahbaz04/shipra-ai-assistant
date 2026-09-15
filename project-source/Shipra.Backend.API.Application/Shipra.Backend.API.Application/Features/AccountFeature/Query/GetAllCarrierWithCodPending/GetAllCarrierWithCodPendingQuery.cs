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

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.GetAllCarrierWithCodPending;
public class GetAllCarrierWithCodPendingQuery : CommonFilterModel,IRequest<ServiceResultDTO>
{
}
public class GetAllCarrierWithCodPendingQueryHandler : RequestHandlerBase<GetAllCarrierWithCodPendingQuery, ServiceResultDTO>
{ 
  private readonly IAccountRepository _accountRepository;

  public GetAllCarrierWithCodPendingQueryHandler(IAccountRepository  accountRepository,IServiceProvider serviceProvider, ILogger<GetAllCarrierWithCodPendingQueryHandler> logger) : base(serviceProvider, logger)
  { 
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCarrierWithCodPendingQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var result = await _accountRepository.GetAllCarrierWithCodPending(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(result);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
