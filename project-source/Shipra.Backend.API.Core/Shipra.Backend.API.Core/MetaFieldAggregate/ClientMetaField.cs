using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ExpenseAggregate;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Shipra.Backend.API.Core.MetaFieldAggregate;
public class ClientMetaField
{
  public int ClientMetaFieldId { get; set; }
  public ClientId? ClientId { get; set; }
  public int? EntityMetaFieldId { get; set; } 
  public string SettingConfig { get; set; } = string.Empty;
  public DateTime? CreatedOn { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public static ClientMetaField CreateClientMetaField(ClientId clientId, int? entityname , string settingconfig)
  {
    return new ClientMetaField()
    {
      ClientId = clientId,
      EntityMetaFieldId = entityname,
      SettingConfig= settingconfig,
      CreatedOn = DateTime.UtcNow,
    };

  }
  public void UpdateclientMetaData(string jsonStr,int? entitytype)
  {
    SettingConfig = jsonStr;
    UpdatedOn = DateTime.UtcNow;
    EntityMetaFieldId = entitytype;
  }
}
