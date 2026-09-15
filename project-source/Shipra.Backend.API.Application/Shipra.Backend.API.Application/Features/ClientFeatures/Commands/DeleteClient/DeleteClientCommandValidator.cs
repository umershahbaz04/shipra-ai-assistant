using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.DeleteProductStation;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.DeleteClient;
public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
{
  public DeleteClientCommandValidator()
  {
    RuleFor(v => v.ClientId).NotNull().NotEmpty();
  }
}
