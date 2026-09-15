using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.ClientAggregate;
public partial class ClientGenericSetting
{
  public int ClientGenericSettingId { get; set; } 
  public ClientId? ClientId { get; set; } 
  public string? SettingConfig { get; set; }

  public static ClientGenericSetting? Create(ClientId clientId, string? settingConfig)
  {
    return new ClientGenericSetting()
    {
      ClientId = clientId,
      SettingConfig = settingConfig
    }; 
  }

  public void UpdateConfig(string? settingConfig)
  {
    SettingConfig = settingConfig;
  }
}
