using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivableStatusUnPaid;

public class UpdateDriverReceivableStatusUnPaidCommand : IRequest<ServiceResultDTO>
{
  public string? DriverReceivableId { get; set; }
}

public class UpdateDriverReceivableStatusUnPaidCommandValidator : AbstractValidator<UpdateDriverReceivableStatusUnPaidCommand>
{
  public UpdateDriverReceivableStatusUnPaidCommandValidator()
  {
    RuleFor(v => v.DriverReceivableId).NotNull().NotEmpty();
  }
}
