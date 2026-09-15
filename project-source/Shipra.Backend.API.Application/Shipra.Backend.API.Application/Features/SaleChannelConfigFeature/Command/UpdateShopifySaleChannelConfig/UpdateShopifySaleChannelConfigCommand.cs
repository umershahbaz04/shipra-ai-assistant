using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.UpdateShopifySaleChannelConfig;
public class UpdateShopifySaleChannelConfigCommand : IRequest<ServiceResultDTO>
{
  public bool? IsActive { get; set; }
  public bool IsAllowToDisplayInSaleChannel { get; set; }
  public Dictionary<string, string>? InputParameters { get; set; }
  public int StoreId { get; set; }
  public int SaleChannelLookupId { get; set; }
  public int SaleChannelConfigId { get; set; }
  public string? SaleChannelName { get; set; }
  public string? AccessToken { get; set; }
  public string? ClientId { get; set; }
  public string? SecretKey { get; set; }
}
