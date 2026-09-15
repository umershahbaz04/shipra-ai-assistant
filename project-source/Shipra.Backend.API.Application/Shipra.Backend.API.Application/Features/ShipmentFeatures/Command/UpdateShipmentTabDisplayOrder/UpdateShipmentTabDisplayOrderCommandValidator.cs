using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.UpdateShipmentTabDisplayOrder;

public class UpdateShipmentTabDisplayOrderCommandValidator : AbstractValidator<UpdateShipmentTabDisplayOrderCommand>
{
  public UpdateShipmentTabDisplayOrderCommandValidator()
  {
    RuleFor(x => x.list).Must(x => x != null);
  }
}
