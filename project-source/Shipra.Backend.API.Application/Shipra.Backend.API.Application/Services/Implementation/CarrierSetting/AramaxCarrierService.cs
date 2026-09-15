using Shipra.Backend.API.Application.Services.Interfaces;

namespace Shipra.Backend.API.Application.Services.Implementation.CarrierSetting;

public class AramaxCarrierService : ICarrierService
{
  public dynamic GetDataByType(string input)
  {
    // Posta-specific logic
    return "Aramax_" + input;
  }
}
