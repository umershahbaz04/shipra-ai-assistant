using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrier;
public class CreateActiveCarrierCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public Dictionary<string, string>? InputParameters { get; set; }
  public List<GeneralSettingConfigModel>? SettingConfig { get; set; } = new();
  public string? CarrierAlias { get; set; }
  public int? CarrierLocationId { get; set; }
  public int? CarrierId { get; set; }
  public int CarrierContractTypeId { get; set; } = (int)EnumCarrierContractType.OwnContractType;
  public decimal? FlatRate { get;  set; } 
}
