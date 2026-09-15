using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.DeleteCpsettlementPopFile;
public class DeleteCpsettlementPopFileCommand : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; }
  public int CpsettlementPopFileId { get; set; }
}
public class DeleteCpsettlementPopFileCommandHandler : RequestHandlerBase<DeleteCpsettlementPopFileCommand, ServiceResultDTO>
{
  private readonly IAccountRepository _accountRepository;

  public DeleteCpsettlementPopFileCommandHandler(IAccountRepository accountRepository, IServiceProvider serviceProvider, ILogger<DeleteCpsettlementPopFileCommandHandler> logger) : base(serviceProvider, logger)
  {
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteCpsettlementPopFileCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var carrierPaymentSettlementId = new CarrierPaymentSettlementId(new Guid(request.CarrierPaymentSettlementId!));

      var oCarrierPaymentSettlement = await _accountRepository.GetCpsettlementPopFile(request.CpsettlementPopFileId, carrierPaymentSettlementId);
      oCarrierPaymentSettlement.Delete(_currentUser.EmployeeId);
      var result = await _accountRepository.DeleteCpsettlementPopFile(oCarrierPaymentSettlement);
      if (result)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = request.CpsettlementPopFileId,
          Message = "File Deleted successfully"
        });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class DeleteCpsettlementPopFileCommandValidator : AbstractValidator<DeleteCpsettlementPopFileCommand>
{
  public DeleteCpsettlementPopFileCommandValidator()
  {
    RuleFor(x => x.CarrierPaymentSettlementId).NotEmpty().NotNull().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(x => x.CpsettlementPopFileId).NotEmpty().NotNull().GreaterThan(0);
  }
}
