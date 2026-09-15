using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.NotificationAggregate;

public  class NotificationEvent
{
  public int NotificationEventId { get; set; } 
  public string? EventName { get; set; }
  public string? EventKey { get; set; }

  public static NotificationEvent AddDefault()
  {
    return new NotificationEvent()
    {
      NotificationEventId = 0,
      EventName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
