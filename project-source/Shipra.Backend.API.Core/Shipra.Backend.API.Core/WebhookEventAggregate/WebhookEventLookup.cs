using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.Core.TaxAggregate;

namespace Shipra.Backend.API.Core.WebhookEventAggregate;

public class WebhookEventLookup
{
  public int WebhookEventLookupId { get; set; } 
  public string? EventName { get; set; } 
  public string? EventKey { get; set; } 
  public string? Description { get; set; } 
  public string? Config { get; set; }
  public bool? Active { get; set; }
  public static WebhookEventLookup AddDefault()
  {
    return new WebhookEventLookup()
    {
      WebhookEventLookupId = 0,
      EventName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
