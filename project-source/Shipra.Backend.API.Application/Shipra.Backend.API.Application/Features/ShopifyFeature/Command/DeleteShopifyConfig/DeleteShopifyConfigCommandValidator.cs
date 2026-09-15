using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ShopifyFeature.Command.DeleteShopifyConfig;
public class DeleteShopifyConfigCommandValidator : AbstractValidator<DeleteShopifyConfigCommand>
{
  public DeleteShopifyConfigCommandValidator()
  {
    RuleFor(v => v.SaleChannelConfigId).NotNull().NotEmpty();
    RuleFor(v => v.ClientId).NotNull().NotEmpty();
    RuleFor(v => v.SecretKey).NotNull().NotEmpty();
  }
}
