using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.GetAllCpsettlementPopFiles;
public class GetAllCpsettlementPopFilesQuery : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; }

}
public class GetAllCpsettlementPopFilesQueryHandler : RequestHandlerBase<GetAllCpsettlementPopFilesQuery, ServiceResultDTO>
{
  private readonly IAccountRepository _accountRepository;

  public GetAllCpsettlementPopFilesQueryHandler(IAccountRepository accountRepository, IServiceProvider serviceProvider, ILogger<GetAllCpsettlementPopFilesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCpsettlementPopFilesQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var carrierPaymentSettlementId = new CarrierPaymentSettlementId(new Guid(request.CarrierPaymentSettlementId!));

      List<CPSettlementPopFile> list = await _accountRepository.GetAllCpsettlementPopFiles(carrierPaymentSettlementId);
      serviceResult = new ServiceResultDTO(list.Select(x => new
      {
        x.CpsettlementPopFileId,
        CarrierPaymentSettlementId = x.CarrierPaymentSettlementId!.Value.ToString(),
        x.FilePath,
        x.Extension
      }).ToList());

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetAllCpsettlementPopFilesQueryValidator : AbstractValidator<GetAllCpsettlementPopFilesQuery>
{
  public GetAllCpsettlementPopFilesQueryValidator()
  {
    RuleFor(x => x.CarrierPaymentSettlementId).NotEmpty().NotNull().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
