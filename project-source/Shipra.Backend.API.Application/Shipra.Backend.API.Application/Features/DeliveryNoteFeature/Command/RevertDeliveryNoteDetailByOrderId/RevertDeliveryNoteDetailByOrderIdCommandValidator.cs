using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.RevertDeliveryNote;
public class RevertDeliveryNoteDetailByOrderIdCommandValidator : AbstractValidator<RevertDeliveryNoteDetailByOrderIdCommand>
{
  public RevertDeliveryNoteDetailByOrderIdCommandValidator()
  {
    RuleFor(v => v.OrderId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
