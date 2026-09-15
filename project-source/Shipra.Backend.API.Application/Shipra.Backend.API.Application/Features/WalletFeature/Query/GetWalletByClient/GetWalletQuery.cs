using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetWalletByClient;
public class GetWalletQuery : IRequest<ServiceResultDTO>
{
}
public class GetWalletQueryHandler : RequestHandlerBase<GetWalletQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetWalletQueryHandler(IWalletRepository walletRepository,IServiceProvider serviceProvider, ILogger<GetWalletQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetWalletQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    { 
      var oWallet = await _walletRepository.GetWalletByClientId(_currentUser.ClientId!);
      if (oWallet == null)
      {
        oWallet = Wallet.CreateWallet(_currentUser.ClientId!,0);
        await _walletRepository.CreateWallat(oWallet);
      }
      dynamic response = await _walletRepository.GetCalucaltedBalance(_currentUser.ClientIdStr); 
      serviceResult = new ServiceResultDTO(response!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
