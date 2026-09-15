using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Services.Implementation.CarrierSetting;
public class UpsCarrierService : ICarrierService
{
  public UpsCarrierService()
  {
  }

  public dynamic GetDataByType(string type)
  {
    // Ups-specific logic
    if (type == "packageCode")
    {
      return GetAllPackageCode();
    }
    else if (type == "serviceCode")
    {
      return GetAllserviceCode();
    }
    else if (type == "ContainerCode")
    {
      return GetAllcontainerCode();
    }
    else if (type == "PickupserviceCode")
    {
      return GetAllserviceCodeForPickup();
    }
    return "Ups_" + type;
  }
  private dynamic GetAllPackageCode()
  {
    return new List<dynamic>
    {
       new { Id = "00", Text = "UNKNOWN" },
       new { Id = "01", Text = "UPS Letter" },
       new { Id = "02", Text = "Package" },
       new { Id = "03", Text = "Tube" },
       new { Id = "04", Text = "Pak" },
       new { Id = "21", Text = "Express Box" },
       new { Id = "24", Text = "25KG Box" },
       new { Id = "25", Text = "10KG Box" },
       new { Id = "30", Text = "Pallet" },
       new { Id = "2a", Text = "Small Express Box" },
       new { Id = "2b", Text = "Medium Express Box" },
       new { Id = "2c", Text = "Large Express Box" }
    };
  }

  private dynamic GetAllserviceCode()
  {
    return new List<dynamic>()
    {
      new { Id = "01", Text = "Next Day Air" },
      new { Id = "02", Text = "2nd Day Air" },
      new { Id = "03", Text = "Ground" },
      new { Id = "12", Text = "3 Day Select" },
      new { Id = "13", Text = "Next Day Air Saver" },
      new { Id = "14", Text = "UPS Next Day Air Early" },
      new { Id = "59", Text = "2nd Day Air A.M." },
      new { Id = "75", Text = "UPS Heavy Goods" },
      new { Id = "07", Text = "Worldwide Express" },
      new { Id = "08", Text = "Worldwide Expedited" },
      new { Id = "11", Text = "Standard" },
      new { Id = "54", Text = "Worldwide Express Plus" },
      new { Id = "65", Text = "Saver" },
      new { Id = "96", Text = "UPS Worldwide Express Freight" },
      new { Id = "71", Text = "UPS Worldwide Express Freight Midday" }
    };
  }

  private dynamic GetAllcontainerCode()
  {
    return new List<dynamic>()
    {
      new { Id = "01", Text = "PACKAGE" },
      new { Id = "02", Text = "UPS LETTER" },
      new { Id = "03", Text = "PALLET" },
    };
  }

  private dynamic GetAllserviceCodeForPickup()
  {
    return new List<dynamic>()
    {
      new { Id = "001", Text = "Next Day Air" },
      new { Id = "002", Text = "2nd Day Air" },
      new { Id = "003", Text = "Ground" },
      new { Id = "012", Text = "3 Day Select" },
      new { Id = "013", Text = "Next Day Air Saver" },
      new { Id = "014", Text = "UPS Next Day Air Early" },
      new { Id = "059", Text = "2nd Day Air A.M." },
    };
  }
}
