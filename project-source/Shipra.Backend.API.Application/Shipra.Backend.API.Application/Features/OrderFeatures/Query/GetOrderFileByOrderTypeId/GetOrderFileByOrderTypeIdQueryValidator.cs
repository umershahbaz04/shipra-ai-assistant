using FluentValidation;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetFullfilableOrderFile;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderFileByOrderTypeId;
public class GetOrderFileByOrderTypeIdQueryValidator : AbstractValidator<GetOrderFileByOrderTypeIdQuery>
{
  public GetOrderFileByOrderTypeIdQueryValidator()
  {
    RuleFor(x => x.OrderTypeId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.CountryId).NotEmpty().NotNull().GreaterThan(0);
  }
}
