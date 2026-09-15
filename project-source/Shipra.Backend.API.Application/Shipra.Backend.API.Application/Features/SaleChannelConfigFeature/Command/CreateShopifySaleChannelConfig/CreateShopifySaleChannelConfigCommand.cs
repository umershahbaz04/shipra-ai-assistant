using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.CreateShopifySaleChannelConfig;
public class CreateShopifySaleChannelConfigCommand : IRequest<ServiceResultDTO>
{
  public bool IsActive { get; set; } = true;
  public bool IsAllowToDisplayInSaleChannel { get; set; } = true;
  public Dictionary<string, string>? InputParameters { get; set; }
  public List<GeneralSettingConfigModel>? SettingConfig { get; set; } = new();
  public int StoreId { get; set; }
  public int SaleChannelLookupId { get; set; }
  public string? SaleChannelName { get; set; }
  public string? AccessToken { get; set; }
  public string? ClientId { get; set; }
  public string? SecretKey { get; set; }
}

public class CreateShopifySaleChannelConfigCommandValidator : AbstractValidator<CreateShopifySaleChannelConfigCommand>
{
  public CreateShopifySaleChannelConfigCommandValidator()
  {
    RuleFor(v => v.SaleChannelName).NotNull().NotEmpty();
    RuleFor(v => v.ClientId).NotNull().NotEmpty();
    RuleFor(v => v.SecretKey).NotNull().NotEmpty();
    RuleFor(v => v.StoreId).NotNull().NotEmpty().GreaterThan(0);
  }
}

