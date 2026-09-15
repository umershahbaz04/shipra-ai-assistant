using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.CreateSaleChannelConfig;
public class CreateSaleChannelConfigCommand : IRequest<ServiceResultDTO>
{
  public bool IsActive { get; set; }
  public bool? IsAllowToDisplayInSaleChannel { get; set; }
  public Dictionary<string, string>? InputParameters { get; set; }
  public List<GeneralSettingConfigModel>? SettingConfig { get; set; } = new();
  public int StoreId { get; set; }
  public int SaleChannelLookupId { get; set; }
  public string? SaleChannelName { get; set; }
  public string? AccessToken { get; set; }
}
