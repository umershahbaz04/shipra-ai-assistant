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
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.GetAllCarrierPaymentSettlements;
public class GetAllCarrierPaymentSettlementsQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllCarrierPaymentSettlementsQueryHandler : RequestHandlerBase<GetAllCarrierPaymentSettlementsQuery, ServiceResultDTO>
{
  private readonly IAccountRepository _accountRepository;

  public GetAllCarrierPaymentSettlementsQueryHandler(IAccountRepository accountRepository,IServiceProvider serviceProvider, ILogger<GetAllCarrierPaymentSettlementsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCarrierPaymentSettlementsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var data = await _accountRepository.GetAllCarrierPaymentSettlements(_currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);

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
public class GetAllCarrierPaymentSettlementsQueryValidator : AbstractValidator<GetAllCarrierPaymentSettlementsQuery>
{
}
