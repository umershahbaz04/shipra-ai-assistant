using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.UpdateProductCategory;
public class UpdateProductCategoryCommandValidtor : AbstractValidator<UpdateProductCategoryCommand>
{
  public UpdateProductCategoryCommandValidtor()
  {
    RuleFor(v => v.ProductCategoryId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.CategoryName).NotNull().NotEmpty();
  }
}
