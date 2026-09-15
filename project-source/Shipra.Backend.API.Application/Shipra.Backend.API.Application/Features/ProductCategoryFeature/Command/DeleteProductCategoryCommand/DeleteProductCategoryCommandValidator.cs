using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.DeleteProductCategoryCommand;
public class DeleteProductCategoryCommandValidator : AbstractValidator<DeleteProductCategoryCommand>
{
  public DeleteProductCategoryCommandValidator()
  {
    RuleFor(v => v.ProductCategoryId).NotNull().NotEmpty().GreaterThan(0);
  }
}
