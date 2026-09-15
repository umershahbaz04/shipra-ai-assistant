using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.NotificationAggregate;
public class NotificationType
{
  public int NotificationTypeId { get; set; } 
  public string? TypeName { get; set; }

  public static NotificationType AddDefault()
  {
    return new NotificationType()
    {
      NotificationTypeId = 0,
      TypeName = ShipraConstants.DropDownPlaceHolderName
    };
  }

}
