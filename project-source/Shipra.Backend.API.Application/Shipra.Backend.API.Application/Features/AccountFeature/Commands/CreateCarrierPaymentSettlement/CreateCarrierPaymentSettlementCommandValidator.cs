using FluentValidation;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.CreateCarrierPaymentSettlement;
public class CreateCarrierPaymentSettlementCommandValidator : AbstractValidator<CreateCarrierPaymentSettlementCommand>
{
  public CreateCarrierPaymentSettlementCommandValidator()
  {
    RuleFor(v => v.CarrierId).NotNull().NotEmpty();
    RuleFor(v => v.list).NotNull().NotEmpty().WithMessage("List must contain at least one item ");
  }
}
