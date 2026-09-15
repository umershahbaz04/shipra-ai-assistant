using FluentValidation;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.DeleteSaleChannelConfig;
public class DeleteSaleChannelConfigCommandValidator : AbstractValidator<DeleteSaleChannelConfigCommand>
{
  public DeleteSaleChannelConfigCommandValidator()
  {
    RuleFor(v => v.SaleChannelConfigId).NotNull().NotEmpty().GreaterThan(0);
  }
}
