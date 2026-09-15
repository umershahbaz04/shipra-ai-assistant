using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ISmsProviderFactory
{
  ISmsProvider CreateSmsProvider(EnumSMSLookup providerType);
}
