using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersByTenantId;
public class GetAllOrdersByTenantIdQueryValidator : AbstractValidator<GetAllOrdersByTenantIdQuery>
{
  public GetAllOrdersByTenantIdQueryValidator()
  {
    RuleFor(v => v.TenantId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
