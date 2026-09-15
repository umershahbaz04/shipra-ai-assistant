using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivableStatus;
public class UpdateDriverReceivableStatusPaidCommand : IRequest<ServiceResultDTO>
{
  public string? DriverReceivableId { get; set; }
}

public class UpdateDriverReceivableStatusCommandValidator : AbstractValidator<UpdateDriverReceivableStatusPaidCommand>
{
  public UpdateDriverReceivableStatusCommandValidator()
  {
    RuleFor(v => v.DriverReceivableId).NotNull().NotEmpty();
  }
}
