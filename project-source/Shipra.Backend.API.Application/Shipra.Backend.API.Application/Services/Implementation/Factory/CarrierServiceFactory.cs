using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.Services.Implementation.CarrierSetting;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Services.Implementation.Factory;
public class CarrierServiceFactory
{
  public ICarrierService Create(int carrierId)
  {
    EnumStaticCarrierType carrier = (EnumStaticCarrierType)carrierId;

    return carrier switch
    {
      EnumStaticCarrierType.Ups => new UpsCarrierService(),
      EnumStaticCarrierType.Aramex => new AramaxCarrierService(),
      _ => throw new NotImplementedException($"Carrier '{carrierId}' is not implemented.")
    };
  }
}
public enum EnumStaticCarrierType
{
  Aramex = 1,
  Ups = 99
  // Add more carriers as needed
}
