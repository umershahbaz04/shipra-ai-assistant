using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.SMSProcessAggregate;

namespace Shipra.Backend.API.Application.Services.Interfaces;
public class SmsService : ISmsService
{
  private readonly ISmsProviderFactory smsProviderFactory;
  private readonly ISMSProcessRepository _smsProcessRepository;

  public SmsService(ISMSProcessRepository sMSProcessRepository, ISmsProviderFactory smsProviderFactory)
  {
    _smsProcessRepository = sMSProcessRepository;
    this.smsProviderFactory = smsProviderFactory;
  }

  public async Task SendSms(Message message, ClientId clientId, int? sMSActivateId = 0)
  {
    try
    {
      var allClientMessages = await _smsProcessRepository.GetAllSmsActivated(clientId!);
      if (allClientMessages?.Count > 0)
      {
        SMSActivate? oSmsActivated = allClientMessages!.FirstOrDefault(x => x.IsDefault == true); ;
        if (sMSActivateId > 0)
        {
          oSmsActivated = allClientMessages!.FirstOrDefault(x => x.SMSActivateId == sMSActivateId);
        } 
        if (oSmsActivated is not null)
        { 
          EnumSMSLookup providerType = (EnumSMSLookup)Enum.ToObject(typeof(EnumSMSLookup), oSmsActivated.SMSLookupId.GetValueOrDefault());

          ISmsProvider smsProvider = smsProviderFactory.CreateSmsProvider(providerType);
          await smsProvider.Send(message, oSmsActivated.Config!);
        }
      }
    }
    catch (Exception ex)
    {
      _ = ex.Message;
      throw;
    }

  }
}
