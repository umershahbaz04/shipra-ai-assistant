using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public class ActiveCarrier
{
  public int ActiveCarrierId { get; private set; }
  public int CarrierId { get; private set; }
  public int? CarrierContractTypeId { get; set; } 
  public ClientId? ClientId { get; private set; }
  public string? Config { get; private set; }
  public string? SettingConfig { get; private set; }
  public string? UserName { get; private set; }
  public string? CarrierAlias { get; private set; }
  public int? RegionTimeZoneId { get; set; }
  public bool? IsWebhookSupported { get; set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public bool? IsActiveCarrier { get; private set; }
  public bool? Active { get; private set; }
  public bool? IsDefault { get; private set; }
  public int? CarrierLocationId { get; set; }

  public static ActiveCarrier CreateActiveCarrier(int carrierId, ClientId clientId, EmployeeId createdBy, int? regionTimeZoneId = null, bool? IsWebhookSupported = null, string? settingConfig = null, bool? isDefault = true, string? config = null, bool? isActiveCarrier = false, string? carrierAlias = null, string? userName = null, int? carrierLocationId = null)
  {
    return new ActiveCarrier()
    {
      CarrierId = carrierId,
      Config = config,
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      IsDefault = isDefault,
      IsActiveCarrier = isActiveCarrier,
      CarrierAlias = carrierAlias,
      SettingConfig = settingConfig,
      UserName = userName,
      RegionTimeZoneId = regionTimeZoneId,
      IsWebhookSupported = IsWebhookSupported,
      CarrierLocationId = carrierLocationId,
      CarrierContractTypeId = (int)EnumCarrierContractType.OwnContractType,
      Active = true
    };
  }

  public void DeActiveCarrier(EmployeeId? employeeId)
  {
    UpdatedBy = employeeId;
    IsActiveCarrier = true;
    Active = false;
  }

  public void UpdateActiveCarrier(int carrierId, ClientId clientId, string config, bool? active, string? carrierAlias, string? userName, EmployeeId updatedById)
  {
    CarrierId = carrierId;
    ClientId = clientId;
    Config = config;
    CarrierAlias = carrierAlias;
    UserName = userName;
    Active = active;
    UpdatedBy = updatedById;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateCarrierAlias(string? carrierAlias, EmployeeId? employeeId)
  {
    CarrierAlias = carrierAlias;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateSettingConfig(string settingConfig,string config, EmployeeId employeeId)
  {
    SettingConfig = settingConfig;
    UpdatedBy = employeeId;
    Config = config;
    UpdatedOn = DateTime.UtcNow;
  }
}
