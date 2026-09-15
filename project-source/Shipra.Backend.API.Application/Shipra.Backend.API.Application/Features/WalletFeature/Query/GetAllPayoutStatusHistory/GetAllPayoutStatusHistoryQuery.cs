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

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllPayoutStatusHistory;
public class GetAllPayoutStatusHistoryQuery : IRequest<ServiceResultDTO>
{
  public string? PayoutId { get; set; }
}
public class GetAllPayoutStatusHistoryQueryHandler : RequestHandlerBase<GetAllPayoutStatusHistoryQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetAllPayoutStatusHistoryQueryHandler(IWalletRepository walletRepository, IServiceProvider serviceProvider, ILogger<GetAllPayoutStatusHistoryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPayoutStatusHistoryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      dynamic data = await _walletRepository.GetAllPayoutStatusHistory(_currentUser.ClientIdStr!, request.PayoutId);
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
