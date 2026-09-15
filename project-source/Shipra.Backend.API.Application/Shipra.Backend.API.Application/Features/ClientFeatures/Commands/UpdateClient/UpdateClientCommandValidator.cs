using FluentValidation;
namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateClient;
public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
 public UpdateClientCommandValidator()
  {
    RuleFor(v => v.Mobile).NotNull().NotEmpty();
    RuleFor(v => v.ClientCompanyName).NotNull().NotEmpty(); 
    //RuleFor(v => v.LicenseNo).NotNull().NotEmpty();
  }
}
