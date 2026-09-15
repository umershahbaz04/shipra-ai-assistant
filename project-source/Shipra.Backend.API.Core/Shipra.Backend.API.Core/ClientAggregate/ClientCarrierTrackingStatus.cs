using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.ClientAggregate;
public class ClientCarrierTrackingStatus
{
  public int ClientCarrierTrackingStatusId { get;private set; }
  public int CarrierTrackingStatusId { get;private set; }
  public string? TrackingStatus { get;private set; }
  public string? TrackingStatusAr { get;private set; }
  public ClientId? ClientId { get;private set; }
  public static ClientCarrierTrackingStatus Create(int carrierTrackingStatusId, string trackingStatus, string trackingStatusAr, ClientId clientId)
  {
    return new ClientCarrierTrackingStatus()
    {
      CarrierTrackingStatusId = carrierTrackingStatusId,
      TrackingStatus = trackingStatus,
      TrackingStatusAr = trackingStatusAr,
      ClientId = clientId
    };
  }
  public static ClientCarrierTrackingStatus AddDefault()
  {
    return new ClientCarrierTrackingStatus()
    {
      CarrierTrackingStatusId = 0,
      TrackingStatusAr = ShipraConstants.DropDownPlaceHolderName
    };
  }

  public static int GetNextCarrierTrackingStatus(List<ClientCarrierTrackingStatus>? allClientCarrierStatus)
  {
    int number = 1;
    if (allClientCarrierStatus!.Count > 0)
    {
      var maxNumber = allClientCarrierStatus!.Max(x => x.CarrierTrackingStatusId);
      number = maxNumber + 1;
    }
    return number;
  }
}
