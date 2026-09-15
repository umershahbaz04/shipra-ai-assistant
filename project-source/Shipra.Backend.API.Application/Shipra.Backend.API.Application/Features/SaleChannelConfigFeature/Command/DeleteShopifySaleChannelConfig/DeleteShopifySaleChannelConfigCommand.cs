using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.DeleteShopifySaleChannelConfig;
public class DeleteShopifySaleChannelConfigCommand : IRequest<ServiceResultDTO>
{
  public int SaleChannelConfigId { get; set; }
  public string? ClientId { get; set; }
  public string? SecretKey { get; set; }
}

public class DeleteShopifySaleChannelConfigCommandValidator : AbstractValidator<DeleteShopifySaleChannelConfigCommand>
{
  public DeleteShopifySaleChannelConfigCommandValidator()
  {
    RuleFor(v => v.SaleChannelConfigId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.ClientId).NotNull().NotEmpty();
    RuleFor(v => v.SecretKey).NotNull().NotEmpty();
  }
}
