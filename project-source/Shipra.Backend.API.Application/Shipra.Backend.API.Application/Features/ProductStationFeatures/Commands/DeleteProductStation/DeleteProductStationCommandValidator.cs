using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.DeleteProductStation;
public class DeleteProductStationCommandValidator : AbstractValidator<DeleteProductStationCommand>
{
  public DeleteProductStationCommandValidator()
  {
    RuleFor(v => v.ProductStationId).NotNull().NotEmpty().GreaterThan(0); 
  }
}
