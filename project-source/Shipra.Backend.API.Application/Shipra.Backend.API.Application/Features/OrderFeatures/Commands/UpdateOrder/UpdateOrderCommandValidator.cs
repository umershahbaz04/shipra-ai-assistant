using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrder;
public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
  public UpdateOrderCommandValidator()
  {
    RuleFor(v => v).NotNull();
    // Validate only when not a draft order
    When(x => x.OrderDraftId.GetValueOrDefault() == 0, () =>
    {
      RuleFor(v => v.OrderId)
          .NotNull()
          .NotEmpty()
          .Must(GuidHelper.Validator)
          .WithMessage(GuidHelper.GuidMessage);

      RuleFor(v => v.StoreId)
          .NotNull()
          .NotEmpty();

      RuleFor(v => v.OrderTypeId)
          .NotNull()
          .NotEmpty();

      RuleFor(v => v.PaymentStatusId)
          .NotNull()
          .NotEmpty();
    });
  }
}
