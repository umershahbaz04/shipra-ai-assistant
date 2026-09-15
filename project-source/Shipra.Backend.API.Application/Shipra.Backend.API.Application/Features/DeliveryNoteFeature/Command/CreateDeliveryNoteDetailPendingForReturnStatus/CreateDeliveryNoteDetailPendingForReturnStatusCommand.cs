using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CreateDeliveryNoteDetailPendingForReturnStatus;
public class CreateDeliveryNoteDetailPendingForReturnStatusCommand : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
}
public class CreateDeliveryNoteDetailPendingForReturnCommandValidator : AbstractValidator<CreateDeliveryNoteDetailPendingForReturnStatusCommand>
{
  public CreateDeliveryNoteDetailPendingForReturnCommandValidator()
  {
    RuleFor(v => v.OrderId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
