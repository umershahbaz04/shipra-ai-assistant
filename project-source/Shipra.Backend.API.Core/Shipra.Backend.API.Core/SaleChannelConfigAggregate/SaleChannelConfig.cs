using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.SaleChannelConfigAggregate;
public class SaleChannelConfig
{
  public int SaleChannelConfigId { get;private set; }
  public int? SaleChannelLookupId { get; private set; }
  public string? SaleChannelKey { get; private set; }
  public int? StoreId { get; private set; }
  public bool? IsSaleChannelActivate { get; private set; }
  public bool? IsAllowToDisplayInSaleChannel { get; private set; }
  public string? Config { get; private set; }
  public string? SettingConfig { get; private set; }
  public string? SaleChannelName { get; private set; }
  public string? UserName { get; private set; }
  public string? Password { get; private set; }
  public ClientId? ClientId { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? UpdateOn { get; private set; }
  public bool? Active { get; private set; }

  public static SaleChannelConfig CreateSaleChannelConfig(int storeId, int saleChannelLookupId, string saleChannelJsonData, string saleChannelName, ClientId? clientId, bool isAllowToDisplayInSaleChannel, EmployeeId? userId,string settingConfig, string? saleChannelKey = null,string? saleChannelUserName = null,string? saleChannelPassword=null)
  {
    return new SaleChannelConfig()
    {
      SaleChannelLookupId = saleChannelLookupId,
      StoreId = storeId,
      SaleChannelName = saleChannelName,
      Config = saleChannelJsonData,
      ClientId = clientId,
      IsSaleChannelActivate = false,
      IsAllowToDisplayInSaleChannel = isAllowToDisplayInSaleChannel,
      SaleChannelKey = saleChannelKey,
      SettingConfig = settingConfig,
      UserName = saleChannelUserName,
      Password = saleChannelPassword,
      CreatedBy = userId,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }
  public static SaleChannelConfig ConverSaleChannelConfig(int? saleChannelConfigId, int? storeId, int? saleChannelLookupId, string saleChannelName, ClientId? clientId, EmployeeId? userId,string? settingConfig, string? saleChannelKey = null,string? saleChannelUserName = null,string? saleChannelPassword=null)
  {
    return new SaleChannelConfig()
    {
      SaleChannelConfigId = saleChannelConfigId.GetValueOrDefault(),
      SaleChannelLookupId = saleChannelLookupId,
      StoreId = storeId,
      SaleChannelName = saleChannelName, 
      ClientId = clientId,
      IsSaleChannelActivate = false, 
      SaleChannelKey = saleChannelKey,
      SettingConfig = settingConfig,
      UserName = saleChannelUserName,
      Password = saleChannelPassword,
      CreatedBy = userId,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public void DeleteSaleChannelConfig(EmployeeId? userId)
  {
    IsSaleChannelActivate = false;
    UpdatedBy = userId;
    UpdateOn = DateTime.UtcNow;
    Active = false;
  }

  public void UpdateSaleChannelConfig(int storeId, string jsonStr, string saleChannelName, bool isActive, bool isAllowToDisplayInSaleChannel, EmployeeId? userId, string settingConfig, string? saleChannelKey = null)
  {
    StoreId = storeId;
    SaleChannelName = saleChannelName;
    Config = jsonStr;
    UpdatedBy = userId;
    IsSaleChannelActivate = false;
    Active = isActive;
    IsAllowToDisplayInSaleChannel = isAllowToDisplayInSaleChannel;
    SaleChannelKey = saleChannelKey;
    UpdateOn = DateTime.UtcNow;
  }
  public void UpdateSaleChannelConfigWhileActivate(EmployeeId? userId)
  {
    UpdatedBy = userId;
    IsSaleChannelActivate = true;
    UpdateOn = DateTime.UtcNow;
  }
}
