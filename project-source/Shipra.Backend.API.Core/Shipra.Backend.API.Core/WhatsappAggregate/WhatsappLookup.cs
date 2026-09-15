using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.Core.SMSProcessAggregate;

namespace Shipra.Backend.API.Core.WhatsappAggregate;
public class WhatsappLookup
{
  public int WhatsAppLookupId { get; set; }
  public string? ServiceName { get; set; }
  public string? InputRequiredConfig { get; set; }
  public string? Config { get; set; }
  public static WhatsappLookup AddDefault()
  {
    return new WhatsappLookup()
    {
      WhatsAppLookupId = 0,
      ServiceName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
