using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetShipmentsByReturnReportId;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.GetShipmentsBySettlementId;
public class GetShipmentsBySettlementIdQuery : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; }
}
public class GetShipmentsBySettlementIdQueryHandler : RequestHandlerBase<GetShipmentsBySettlementIdQuery, ServiceResultDTO>
{
  private readonly IAccountRepository _accountRepository;

  public GetShipmentsBySettlementIdQueryHandler(IAccountRepository accountRepository,IServiceProvider serviceProvider, ILogger<GetShipmentsBySettlementIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetShipmentsBySettlementIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var clientId = _currentUser.ClientId!.Value.ToString();

      dynamic data = await _accountRepository.GetShipmentsBySettlementId(request.CarrierPaymentSettlementId!, clientId);

      serviceResult = new ServiceResultDTO(data);
      serviceResult.CreateSuccessResponse();
      return serviceResult;

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetShipmentsBySettlementIdQueryValidator : AbstractValidator<GetShipmentsBySettlementIdQuery>
{
  public GetShipmentsBySettlementIdQueryValidator()
  {
    RuleFor(x => x.CarrierPaymentSettlementId).NotNull().NotEmpty();
  }
}
