using FluentValidation;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelOrderPreProcessor;
public class SaleChannelOrderPreProcessorCommandValidator : AbstractValidator<SaleChannelOrderPreProcessorCommand>
{
  public SaleChannelOrderPreProcessorCommandValidator()
  {
    RuleFor(v => v.StoreId).NotNull().NotEmpty();
    RuleFor(v => v.SaleChannelConfigId).NotNull().NotEmpty();
    RuleFor(v => v.CreatedFrom).NotNull().NotEmpty();
    RuleFor(v => v.CreatedTo).NotNull().NotEmpty();
  }
}
