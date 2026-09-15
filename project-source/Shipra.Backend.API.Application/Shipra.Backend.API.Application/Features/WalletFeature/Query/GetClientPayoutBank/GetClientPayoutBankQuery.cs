using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.Record;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetClientPayoutBank;
public class GetClientPayoutBankQuery : IRequest<ServiceResultDTO>
{
}
public class GetClientPayoutBankQueryHandler : RequestHandlerBase<GetClientPayoutBankQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetClientPayoutBankQueryHandler(IWalletRepository walletRepository, IServiceProvider serviceProvider, ILogger<GetClientPayoutBankQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientPayoutBankQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oWallet = await _walletRepository.GetClientPayoutBank(_currentUser.ClientId!);

      if (oWallet is null)
      {
        throw new EntityNotFoundException("PayoutBank ", _currentUser.ClientIdStr!);
      }
      var data = new
      {
        ClientPayoutBankId = oWallet!.ClientPayoutBankId!.Value!.ToString(),
        oWallet.BankName,
        oWallet.AccountTitle,
        oWallet.Iban,
        oWallet.SwiftCode,
        oWallet.BranchName,
      };
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
