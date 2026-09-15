using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.DeliveryTaskAggregate;
public partial class DeliveryTaskStatusLookup
{
  public int DeliveryTaskStatusId { get; set; } 
  public string? DeliveryTaskStatus { get; set; }

  public static DeliveryTaskStatusLookup AddDefault()
  {
    return new DeliveryTaskStatusLookup()
    {
      DeliveryTaskStatusId = 0,
      DeliveryTaskStatus = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
