using FluentValidation;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelLookupById;
public class GetSaleChannelLookupByIdQueryValidator : AbstractValidator<GetSaleChannelLookupByIdQuery>
{
  public GetSaleChannelLookupByIdQueryValidator()
  {
    RuleFor(v => v.SaleChannelLookupId).NotNull().NotEmpty().GreaterThan(0);
  }
}
