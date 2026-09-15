using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.CreateShipmentGridColumn;

public class CreateShipmentGridColumnCommandValidator : AbstractValidator<CreateShipmentGridColumnCommand>
{
  public CreateShipmentGridColumnCommandValidator()
  {
    RuleFor(x => x.ColumnName).NotNull().NotEmpty();
    RuleFor(x => x.DashboardStatusIdValues).NotNull().NotEmpty();

  }
}
