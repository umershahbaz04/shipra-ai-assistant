using FluentValidation;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelByStoreIdForSelection;
public class GetSaleChannelByStoreIdForSelectionQueryValidator : AbstractValidator<GetSaleChannelByStoreIdForSelectionQuery>
{
  public GetSaleChannelByStoreIdForSelectionQueryValidator()
  {
    RuleFor(v => v.StoreId).NotNull().NotEmpty().GreaterThan(0);
  }
}
