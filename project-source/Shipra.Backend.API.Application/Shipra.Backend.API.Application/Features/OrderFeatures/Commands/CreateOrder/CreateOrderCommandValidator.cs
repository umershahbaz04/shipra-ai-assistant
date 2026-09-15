using FluentValidation;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
  public CreateOrderCommandValidator()
  {
    RuleFor(x => x.orderList).Must(x => x != null).WithMessage("OrderItems list must contain at least one item.");
    RuleForEach(x => x.orderList).SetValidator(x => new CreateOrderCommandRequestModelValidator()).When(x => x.OrderDraftId == 0); ;
  }
}
public class CreateOrderCommandRequestModelValidator : AbstractValidator<CreateOrderRequestModel>
{
  public CreateOrderCommandRequestModelValidator()
  {
    RuleFor(v => v.OrderTypeId).NotNull().GreaterThan(0);
    RuleFor(v => v.OrderDate).NotNull().NotEmpty();
    RuleFor(x => x.OrderItems)
    .Must(x => x != null && x.Count > 0)
    .WithMessage("OrderItems list must contain at least one item.");
    //RuleFor(x => x.OrderAddress).Must(x => x != null).WithMessage("OrderAddress cannot be null.");
    //RuleFor(x => x.Amount).NotNull().GreaterThan(0);
    //When(v => v.PaymentMethodId == (int)EnumPaymentMethod.COD, () =>
    //{
    //  RuleFor(x => x.Amount).NotNull().GreaterThan(0);
    //});
    //validate order items
    RuleForEach(x => x.OrderItems).SetValidator(x => new CreateOrderItemValidator(x.OrderTypeId));

    #region order address
    RuleFor(v => v.OrderAddress!.CountryId!).NotNull().NotEmpty().GreaterThan(0);
    //RuleFor(v => v.OrderAddress!.CityId!).NotNull().NotEmpty().GreaterThan(0);
    //RuleFor(v => v.OrderAddress!.CityId!).NotNull().NotEmpty().GreaterThan(0);
    //RuleFor(v => v.OrderAddress!.StreetAddress!).NotNull().NotEmpty();
    RuleFor(v => v.OrderAddress!.CustomerName!).NotNull().NotEmpty();
    RuleFor(v => v.OrderAddress!.Mobile1!).NotNull().NotEmpty();
     
    #endregion
  }
}
public class CreateOrderItemValidator : AbstractValidator<OrderItemModel>
{
  public CreateOrderItemValidator(int? orderTypeId)
  {
    RuleFor(v => v.Quantity).NotNull().NotEmpty().GreaterThan(0);

    When(v => orderTypeId == (int)EnumOrderType.FullFilable, () =>
    { 
      RuleFor(v => v.ProductId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
      // ProductStockId is now optional
    });

  }
}
