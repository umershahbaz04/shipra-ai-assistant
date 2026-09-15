using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.UpdateOrderStatusOnDebrief;

public class UpdateOrderStatusForCompleteOnDebriefCommand : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteDetailId { get; set; }

}

public class UpdateOrderStatusOnDebriefCommandValidator : AbstractValidator<UpdateOrderStatusForCompleteOnDebriefCommand>
{
  public UpdateOrderStatusOnDebriefCommandValidator()
  {
    RuleFor(v => v.DeliveryNoteDetailId).NotNull().NotEmpty();
  }
}
