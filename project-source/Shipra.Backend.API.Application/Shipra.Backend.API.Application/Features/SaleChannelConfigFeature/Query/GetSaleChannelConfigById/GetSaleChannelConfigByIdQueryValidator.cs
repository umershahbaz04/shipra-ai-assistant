using FluentValidation;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigById;
public class GetSaleChannelConfigByIdQueryValidator : AbstractValidator<GetSaleChannelConfigByIdQuery>
{
  public GetSaleChannelConfigByIdQueryValidator()
  {
    RuleFor(v => v.SaleChannelConfigId).NotNull().NotEmpty().GreaterThan(0);
  }
}
