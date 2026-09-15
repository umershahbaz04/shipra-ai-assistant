using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ShopifyFeature.Command.CreateShopifyConfig;
public class CreateShopifyConfigCommandValidator : AbstractValidator<CreateShopifyConfigCommand>
{
  public CreateShopifyConfigCommandValidator()
  {
    RuleFor(v => v.shop).NotNull().NotEmpty();
    RuleFor(v => v.code).NotNull().NotEmpty();
  }
}
