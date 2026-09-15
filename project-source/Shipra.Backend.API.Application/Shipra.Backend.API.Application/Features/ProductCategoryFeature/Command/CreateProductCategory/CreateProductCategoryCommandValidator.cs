using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.CreateProductCategory;
public class CreateProductCategoryCommandValidator : AbstractValidator<CreateProductCategoryCommand>
{
  public CreateProductCategoryCommandValidator()
  {
    RuleFor(v => v.CategoryName).NotNull().NotEmpty();
  }
}
