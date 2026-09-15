using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllClientWallet;
public class GetAllClientWalletQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllClientWalletQueryHandler : RequestHandlerBase<GetAllClientWalletQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetAllClientWalletQueryHandler(IWalletRepository walletRepository,IServiceProvider serviceProvider, ILogger<GetAllClientWalletQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientWalletQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _walletRepository.GetAllClientWallets(_currentUser.ClientIdStr!);
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
