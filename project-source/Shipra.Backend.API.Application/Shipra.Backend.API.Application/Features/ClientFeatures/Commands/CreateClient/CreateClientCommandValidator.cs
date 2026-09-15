using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateClient;
public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
  public CreateClientCommandValidator()
  {
    RuleFor(v => v.ClientName).NotNull().NotEmpty();
    RuleFor(v => v.Mobile).NotNull().NotEmpty();
    //RuleFor(v => v.CountryId).NotNull().NotEmpty().GreaterThan(0);
    //RuleFor(v => v.RegionId).NotNull().NotEmpty().GreaterThan(0);
    //RuleFor(v => v.CityId).NotNull().NotEmpty().GreaterThan(0);
    //RuleFor(v => v.StreetAddress).NotNull().NotEmpty();

    //client user information
    RuleFor(v => v.Email).NotEmpty().WithMessage("Email address is required").EmailAddress().WithMessage("A valid email is required");
    RuleFor(v => v.UserName).NotNull().NotEmpty();
    RuleFor(v => v.Password).NotNull().NotEmpty()
      .MinimumLength(8).WithMessage("Your password length must be at least 8.")
      .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
      .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
      .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
      .Matches(@"[^A-Za-z0-9]+").WithMessage("Your password must contain special chracter.");
  }
}
#region Details

#endregion


