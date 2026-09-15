using FluentValidation;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelProductPostProcessor;
public class SaleChannelProductPostProcessorCommandValidator : AbstractValidator<SaleChannelProductPostProcessorCommand>
{
  public SaleChannelProductPostProcessorCommandValidator()
  {
    RuleFor(v => v.ProductIds).NotEmpty().When(v => string.IsNullOrEmpty(v.Skus));
    RuleFor(v => v.Skus).NotEmpty().When(v => string.IsNullOrEmpty(v.ProductIds));
  }
}
