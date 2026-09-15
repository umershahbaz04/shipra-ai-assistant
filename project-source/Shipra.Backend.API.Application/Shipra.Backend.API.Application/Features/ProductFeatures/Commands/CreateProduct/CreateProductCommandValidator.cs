using FluentValidation;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateProduct;
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
  public CreateProductCommandValidator()
  {
    RuleFor(v => v.SKU).NotNull().NotEmpty();
    RuleFor(v => v.ProductName).NotNull().NotEmpty();
    RuleFor(v => v.ProductCategoryId).NotNull().NotEmpty().GreaterThan(0);
      
    When(v => v.HaveOptions == true, () =>
    {
      RuleFor(x => x.ProductOptions).Must(x => x != null).WithMessage("Product Options list must contain at least one item.");
      RuleForEach(model => model.ProductOptions).SetValidator(model => new CreateProductOptionsValidator());
    });

    RuleFor(x => x.ProductStocks).Must(x => x != null).WithMessage("Product Stocks list must contain at least one item.");
    RuleForEach(model => model.ProductStocks).SetValidator(model => new CreateProductStocksValidator(model.HaveOptions));

    //RuleFor(v => v.Price).NotNull().NotEmpty();
    //RuleFor(v => v.Description).NotNull().NotEmpty();
    //RuleFor(v => v.QuantityAvailable).NotNull().NotEmpty();
    //// RuleFor(v => v.ClientId).NotNull().NotEmpty();

    //RuleFor(x => x.ProductOptions).Must(x => x != null).WithMessage("Product Options list must contain at least one item.");
    //RuleFor(x => x.ProductStocks).Must(x => x != null).WithMessage("Product Stocks list must contain at least one item.");

    //RuleForEach(model => model.ProductOptions).SetValidator(model => new CreateProductOptionsValidator());
    //RuleForEach(model => model.ProductStocks).SetValidator(model => new CreateProductStocksValidator());
  }
}
public class CreateProductOptionsValidator : AbstractValidator<ProductOptionReuqestModel>
{
  public CreateProductOptionsValidator()
  {
    RuleFor(v => v.OptionValue).NotNull().NotEmpty();
    RuleFor(v => v.OptionId).NotNull().NotEmpty();
  }
}
public class CreateProductStocksValidator : AbstractValidator<ProductStockReuqestModel>
{
  public CreateProductStocksValidator(bool haveOptions)
  {
    RuleFor(v => v.Sku).NotNull().NotEmpty();
    if (haveOptions)
    { 
      RuleFor(v => v.VarientOption).NotNull().NotEmpty();
    }
  }
}
