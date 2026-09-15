using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.MoveStatusFromOneTabToAnother;

public class MoveDashboardStatusFromOneTabToAnotherCommandValidator : AbstractValidator<MoveDashboardStatusFromOneTabToAnotherCommand>
{
  public MoveDashboardStatusFromOneTabToAnotherCommandValidator()
  {
    RuleFor(x => x.ShipmentGridColumnId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(x => x.CarrierTrackingStatusId).NotNull().NotEmpty().GreaterThan(0); 
  }
}
