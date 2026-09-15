using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.MetaFieldAggregate;
public class MetaField
{
  public int MetaFieldId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? EntityId { get; set; }
  public string? SettingConfig { get; set; }
  public DateTime? CreatedOn { get; set; }
  public DateTime? UpdatedOn { get; set; }

  public static MetaField CreateMetaField(string settingconfig,string entitytypeid,ClientId clientId)
  {
    return new MetaField()
    {
      SettingConfig = settingconfig,
      EntityId = entitytypeid,
      ClientId = clientId,
      CreatedOn = DateTime.UtcNow,
    };

  }
  public void UpdateMetaField(string jsonStr)
  {
    SettingConfig = jsonStr;
    UpdatedOn = DateTime.UtcNow;
  }
}
