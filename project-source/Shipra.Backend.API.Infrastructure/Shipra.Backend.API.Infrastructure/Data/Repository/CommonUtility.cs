using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class CommonUtility
{
  public static async Task<int> GetClientRegionMinutes(string? clientId, AppDbContext _context)
  {
    int minuts = (int)EnumRegionTimeZoneMinut.GulfStandartTime;
    if (!string.IsNullOrEmpty(clientId))
    {
      ClientId? client = new ClientId(new Guid(clientId));
      var oClient = await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == client);
      if (oClient != null)
      {
        var oRegionTimeZones = await _context.RegionTimeZones.FirstOrDefaultAsync(x => x.RegionTimeZoneId == oClient.RegionTimeZoneId);
        if (oRegionTimeZones is not null)
        {
          minuts = oRegionTimeZones.Minutes.GetValueOrDefault();
        }
      }
    }
    return minuts;
  }
  public static string GetFormatedDateStr(string date, int regionMinuts)
  {
    return $"DATEADD(MINUTE, {regionMinuts}, {date})";
  }

  public static async Task<string> GetClientGenericSettingValue(string clientId, string sectionKey, string itemKey, AppDbContext context)
  {
    if (string.IsNullOrEmpty(clientId)) return "";
    try
    {
      var clientGuid = new ClientId(new Guid(clientId));
      var genericSetting = await context.ClientGenericSettings.FirstOrDefaultAsync(x => x.ClientId == clientGuid);
      if (genericSetting != null && !string.IsNullOrEmpty(genericSetting.SettingConfig))
      {
        var configList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Shipra.Backend.API.Core.Models.GeneralSettingConfigModel>>(genericSetting.SettingConfig);
        var section = configList?.FirstOrDefault(x => x.Key == sectionKey);
        var item = section?.InputData?.FirstOrDefault(x => x.Key == itemKey);
        return item?.Value ?? "";
      }
    }
    catch
    {
      // Ignore parser errors
    }
    return "";
  }
}
