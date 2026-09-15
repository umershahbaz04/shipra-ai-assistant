using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.MetaFieldAggregate;
public class EntityMetaFieldLookup
{
  public int EntityMetaFieldId { get; set; }
  public string? SettingConfig { get; set; }
  public string? TypeName { get; set; }
}
