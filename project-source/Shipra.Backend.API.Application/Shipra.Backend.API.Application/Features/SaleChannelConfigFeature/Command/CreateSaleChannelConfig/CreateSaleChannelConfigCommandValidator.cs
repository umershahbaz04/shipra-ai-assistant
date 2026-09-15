using FluentValidation;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.CreateSaleChannelConfig;
public class CreateSaleChannelConfigCommandValidator : AbstractValidator<CreateSaleChannelConfigCommand>
{
  public CreateSaleChannelConfigCommandValidator()
  {
    RuleFor(v => v.StoreId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.SaleChannelName).NotNull().NotEmpty();

    //When(v => v.SaleChannelLookupId == (int)EnumSaleChannelLookup.SalePerson, () =>
    //{
    //  RuleFor(v => v.SettingConfig).NotNull().NotEmpty();
    //});
  }
}
