using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.Models;
public class SaleChannelConfigResponseModel
{
  public int SaleChannelConfigId { get; set; }
  public int? SaleChannelLookupId { get; set; }
  public string? SaleChannelKey { get; set; }
  public int? StoreId { get; set; }
  public bool? IsSaleChannelActivate { get; set; }
  public bool? IsAllowToDisplayInSaleChannel { get; set; }
  public string? Config { get; set; }
  public string? SCSettingConfig { get; set; }
  public string? SaleChannelName { get; set; }
  public string? UserName { get; set; }
  public string? Password { get; set; }
   
  public string? InputRequiredConfig { get; set; }
  public string? ImageUrl { get; set; }
  public string? SCLSettingConfig { get; set; }
  public string? AppUrl { get; set; }
  public Guid? ClientId { get; set; }
  public Guid? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public Guid? UpdatedBy { get; set; }
  public DateTime? UpdateOn { get; set; }
  public bool? Active { get; set; }
}
