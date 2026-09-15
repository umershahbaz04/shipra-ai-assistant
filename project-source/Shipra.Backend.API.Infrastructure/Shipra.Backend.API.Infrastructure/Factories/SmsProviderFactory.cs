using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Infrastructure.Services.Sms;

namespace Shipra.Backend.API.Infrastructure.Factories;
public class SmsProviderFactory : ISmsProviderFactory
{
  public ISmsProvider CreateSmsProvider(EnumSMSLookup providerType)
  {
    switch (providerType)
    {
      case EnumSMSLookup.SmartSms:
        return new SmartSMSService();
      case EnumSMSLookup.CountrySms:
        return new CountrySMSService();
      // Add more cases as needed for additional providers
      default:
        throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null);
    }
  }
}
