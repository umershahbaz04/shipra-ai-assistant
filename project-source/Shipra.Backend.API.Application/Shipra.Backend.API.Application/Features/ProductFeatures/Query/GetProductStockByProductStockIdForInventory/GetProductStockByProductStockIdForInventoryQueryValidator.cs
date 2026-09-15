using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductStockByProductStockId;
public class GetProductStockByProductStockIdForInventoryQueryValidator : AbstractValidator<GetProductStockByProductStockIdForInventoryQuery>
{
  public GetProductStockByProductStockIdForInventoryQueryValidator()
  {
    RuleFor(v => v.ProductStockId).NotNull().NotEmpty();
  }
}


