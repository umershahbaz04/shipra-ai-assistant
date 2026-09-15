using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public class ShipraContractCarrier
{
  public int ShipraContractCarrierId { get; private set; }
  public int CarrierId { get; private set; }
  public int? CarrierContractTypeId { get; set; }
  public decimal? FlatRate { get; private set; }
  public string? Config { get; private set; }
  public string? SettingConfig { get; private set; }
  public string? UserName { get; private set; }
  public string? CarrierAlias { get; private set; }
  public int? RegionTimeZoneId { get; set; }
  public bool? IsWebhookSupported { get; set; }
  public DateTime? CreatedOn { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public bool? Active { get; private set; }

  public static ShipraContractCarrier CreateShipraContractCarrier(decimal flatRate, int carrierId,int? regionTimeZoneId = null, bool? IsWebhookSupported = null, string? settingConfig = null,string? config = null, string? carrierAlias = null, string? userName = null)
  {
    return new ShipraContractCarrier()
    {
      CarrierId = carrierId,
      Config = config,
      CreatedOn = DateTime.UtcNow,  
      CarrierAlias = carrierAlias,
      SettingConfig = settingConfig,
      UserName = userName,
      FlatRate = flatRate,
      RegionTimeZoneId = regionTimeZoneId,
      IsWebhookSupported = IsWebhookSupported,
      CarrierContractTypeId = (int)EnumCarrierContractType.ShipraContractType, 
      Active = true
    };
  }
  public void UpdateCarrierAlias(string? carrierAlias)
  {
    CarrierAlias = carrierAlias; 
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateSettingConfig(string settingConfig,string? config,decimal? flatRate)
  {
    SettingConfig = settingConfig; 
    FlatRate = flatRate;
    Config = config;
    UpdatedOn = DateTime.UtcNow;
  }
  //Update price ....
  public void UpdateCarrierPrice(decimal? flatRate)
  {
    
    FlatRate = flatRate;
    UpdatedOn = DateTime.UtcNow;
  }
  public void ActivateCarrier()
  {
    Active = true;
  }
  public void DeActivateCarrier()
  {
    Active = false;
  }
}
