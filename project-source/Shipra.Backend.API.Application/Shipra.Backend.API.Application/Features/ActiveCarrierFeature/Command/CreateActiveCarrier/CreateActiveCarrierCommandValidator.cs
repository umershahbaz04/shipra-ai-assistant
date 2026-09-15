using FluentValidation;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrier;
public class CreateActiveCarrierCommandValidator : AbstractValidator<CreateActiveCarrierCommand>
{
  public CreateActiveCarrierCommandValidator()
  {
    //RuleFor(v => v.InputParameters).NotNull().NotEmpty();
    RuleFor(v => v.CarrierId).NotNull().GreaterThan(0);
    When(v => v.CarrierContractTypeId > 0 && v.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType, () =>
    {
      RuleFor(x => x.FlatRate).NotNull().NotEmpty().GreaterThan(0);
    });
  }
}
