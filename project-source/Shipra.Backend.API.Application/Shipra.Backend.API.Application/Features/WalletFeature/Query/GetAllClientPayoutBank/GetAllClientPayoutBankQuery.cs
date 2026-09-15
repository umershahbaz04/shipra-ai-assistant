using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllClientPayoutBank;
public class GetAllClientPayoutBankQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllClientPayoutBankQueryHandler : RequestHandlerBase<GetAllClientPayoutBankQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetAllClientPayoutBankQueryHandler(IWalletRepository walletRepository,IServiceProvider serviceProvider, ILogger<GetAllClientPayoutBankQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientPayoutBankQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    { 
      var data = await _walletRepository.GetAllClientPayoutBank(_currentUser.ClientIdStr!);
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
