using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.BatchUpdateOrderStatus;
public class BatchUpdateOrderStatusCommandValidator : AbstractValidator<BatchUpdateOrderStatusCommand>
{
  public BatchUpdateOrderStatusCommandValidator()
  {
    RuleFor(v => v.OrderNos).NotNull().NotEmpty();
    RuleFor(v => v.CarrierStatusId).NotNull().NotEmpty().GreaterThan(0);
  }
}

