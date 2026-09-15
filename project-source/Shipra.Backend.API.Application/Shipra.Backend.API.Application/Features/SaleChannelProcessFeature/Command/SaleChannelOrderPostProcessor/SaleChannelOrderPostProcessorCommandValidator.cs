using FluentValidation;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelOrderPostProcessor;
public class SaleChannelOrderPostProcessorCommandValidator : AbstractValidator<SaleChannelOrderPostProcessorCommand>
{
  public SaleChannelOrderPostProcessorCommandValidator()
  {
    RuleFor(x => x.OrderIds).NotNull().NotEmpty();
  }
}
