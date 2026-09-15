using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderFromSaleChannelOrders;
public class CreateOrderFromSaleChannelOrdersCommandValidator : AbstractValidator<CreateOrderFromSaleChannelOrdersCommand>
{
  public CreateOrderFromSaleChannelOrdersCommandValidator()
  {
    RuleFor(v => v.OrderIds).NotNull().NotEmpty();
    RuleFor(v => v.SaleChannelLookupId).NotNull().NotEmpty().GreaterThan(0);
  }
}
