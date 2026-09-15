using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.ClientAggregate;
public class ClientConfigSetting
{
  public int ClientConfigSettingId { get; set; }
  public ClientId? ClientId { get; set; }
  public bool? AutoOrderStatusUpdate { get; set; }
  public bool? AllowShipperInvocie { get; set; }
  public int? RefreshOrderMinut { get; set; }
  public bool? AutoCreateDeliveryTask { get; set; }

  public static ClientConfigSetting Create(ClientId? clientId, bool? autoOrderStatusUpdate, int refreshOrderMinut)
  {
    return new ClientConfigSetting()
    {
      ClientId = clientId,
      AutoOrderStatusUpdate = autoOrderStatusUpdate,
      RefreshOrderMinut = refreshOrderMinut
    };
  }

  public void UpdateAutoStatusUpdateSetting(bool? autoOrderStatusUpdate, int refreshOrderMinut)
  {
    AutoOrderStatusUpdate = autoOrderStatusUpdate;
    RefreshOrderMinut = refreshOrderMinut;
  } 
  public void UpdateShipperInvoiceSetting(bool? allow = false)
  { 
    AllowShipperInvocie = allow;
  }
}
