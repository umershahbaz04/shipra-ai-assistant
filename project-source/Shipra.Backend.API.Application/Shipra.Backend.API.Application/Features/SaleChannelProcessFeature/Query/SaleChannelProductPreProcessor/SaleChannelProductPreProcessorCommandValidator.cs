using FluentValidation;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelProductPreProcessor;
public class SaleChannelProductPreProcessorCommandValidator : AbstractValidator<SaleChannelProductPreProcessorCommand>
{
  public SaleChannelProductPreProcessorCommandValidator()
  {
    RuleFor(v => v.StoreId).NotNull().NotEmpty();
    RuleFor(v => v.SaleChannelConfigId).NotNull().NotEmpty();
    RuleFor(v => v.CreatedFrom).NotNull().NotEmpty();
    RuleFor(v => v.CreatedTo).NotNull().NotEmpty();
  }
}
