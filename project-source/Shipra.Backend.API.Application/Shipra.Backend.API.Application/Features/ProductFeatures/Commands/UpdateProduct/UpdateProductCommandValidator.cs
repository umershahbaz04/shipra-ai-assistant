using FluentValidation;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UpdateProduct;
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
  public UpdateProductCommandValidator()
  {
    RuleFor(v => v.Sku).NotNull().NotEmpty();
    RuleFor(v => v.Price).NotNull().NotEmpty();
    RuleFor(v => v.Description).NotNull().NotEmpty();
    RuleFor(v => v.QuantityAvailable).NotNull().NotEmpty();

    RuleFor(x => x.ProductOptions).Must(x => x != null).WithMessage("Product Options list must contain at least one item.");
    RuleFor(x => x.ProductStocks).Must(x => x != null).WithMessage("Product Stocks list must contain at least one item.");

    RuleForEach(model => model.ProductOptions).SetValidator(model => new UpdateProductOptionsValidator());
    //RuleForEach(model => model.ProductStocks).SetValidator(model => new UpdateProductStocksValidator());
  }
}

public class UpdateProductOptionsValidator : AbstractValidator<ProductOptionReuqestModel>
{
  public UpdateProductOptionsValidator()
  {
    RuleFor(v => v.OptionValue).NotNull().NotEmpty();
    RuleFor(v => v.OptionId).NotNull().NotEmpty();
  }
}
public class UpdateProductStocksValidator : AbstractValidator<ProductStockReuqestModel>
{
  public UpdateProductStocksValidator()
  {
    RuleFor(v => v.ProductStationId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.Sku).NotNull().NotEmpty();
    RuleFor(v => v.VarientOption).NotNull().NotEmpty();
    RuleFor(v => v.QuantityAvailable).NotNull().NotEmpty();//.GreaterThan(0); 
  }
}
