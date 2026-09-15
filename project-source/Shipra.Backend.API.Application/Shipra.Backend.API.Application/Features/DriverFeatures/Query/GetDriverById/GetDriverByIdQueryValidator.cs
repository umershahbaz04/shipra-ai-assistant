using FluentValidation;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetDriverById;
public class GetDriverByIdQueryValidator : AbstractValidator<GetDriverByIdQuery>
{
  public GetDriverByIdQueryValidator()
  {
    RuleFor(v => v.DriverId).NotEmpty().NotNull();
  }
}
