using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrderItems;
public class DeleteOrderItemCommandValidator : AbstractValidator<DeleteOrderItemByIdCommand>
{
  public DeleteOrderItemCommandValidator()
  {
    RuleFor(v => v.OrderId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(v => v.OrderItemId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
