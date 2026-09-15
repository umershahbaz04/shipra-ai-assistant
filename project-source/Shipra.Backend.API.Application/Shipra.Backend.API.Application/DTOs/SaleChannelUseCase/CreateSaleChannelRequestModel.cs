using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase;
public class CreateSaleChannelRequestModel
{
  public EmployeeId? EmployeeId { get; set; }
  public ClientId? ClientId { get; set; }
  public ShopifyConfig? ShopifyConfig { get; set; }
  public SaleChannelConfig? SaleChannelConfig { get; set; }
}
