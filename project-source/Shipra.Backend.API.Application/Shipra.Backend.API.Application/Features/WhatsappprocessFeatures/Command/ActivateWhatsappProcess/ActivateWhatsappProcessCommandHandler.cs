using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.ActivateSMSProcess;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SMSProcessAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Command.ActivateWhatsappProcess;
public class ActivateWhatsappProcessCommandHandler : RequestHandlerBase<ActivateWhatsappProcessCommand, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;
  public ActivateWhatsappProcessCommandHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<ActivateWhatsappProcessCommandHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(ActivateWhatsappProcessCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var dict = Utils.ConvertKeysToCamelCase(request.InputParameters!);
      var WhatsapplookupId = Utils.GetValueFromDictionryByKey("whatsapplookupId", dict);
      if (!string.IsNullOrEmpty(WhatsapplookupId))
      {
        var Whatsapplookup = await _smsProcessRepository.GetWhatsappLookupById(int.Parse(WhatsapplookupId));
        #region exist SMSlookup and config check
        if (Whatsapplookup is null)
        {
          throw new EntityNotFoundException("WhatsappLookup ", int.Parse(WhatsapplookupId));
        }
        if (string.IsNullOrEmpty(Whatsapplookup!.Config!))
        {
          throw new EntityNotFoundException("WhatsappProcessConfig ", Whatsapplookup!.Config!);
        }

        #endregion
        // var carrierDic = Utils.ConvertKeysToCamelCase(JsonConvert.DeserializeObject<Dictionary<string, string>>(Whatsapplookup!.Config!)!,);
        var carrierDic = new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(Whatsapplookup!.Config!)!, StringComparer.OrdinalIgnoreCase);
        #region update config model
        foreach (KeyValuePair<string, string> entry in dict)
        {
          // do something with entry.Value or entry.Key 
          if (!string.IsNullOrEmpty(Utils.GetValueFromDictionryByKey(entry.Key, carrierDic)))
          {
            carrierDic[entry.Key] = entry.Value;
          }
        }
        #endregion

        var jsonStr = JsonConvert.SerializeObject(carrierDic, Formatting.Indented);
        var Whatsappactivate = await _smsProcessRepository.GetWhatsappActivateById(Whatsapplookup.WhatsAppLookupId, _currentUser.ClientId!);

        //get all activated payment process
        var allDefaultWhatsappActivated = await _smsProcessRepository.GetAllDefaultWhatsappActivated(_currentUser.ClientId!);

        #region get all client sms if result 0 then make it defualt
        var allClientMessages = await _smsProcessRepository.GetAllWhatsappActivated(_currentUser.ClientId!);
        if (allClientMessages!.Count == 0 || !allClientMessages.Any(x => x.IsDefault == true))
        {
          request.IsDefault = true;
        }
        #endregion

        if (Whatsappactivate == null)
        {
          Whatsappactivate = WhatsappActivate.CreateWhatsappActivate(Whatsapplookup.WhatsAppLookupId, jsonStr, _currentUser.ClientId, _currentUser.EmployeeId, request.IsDefault);
          Whatsappactivate = await _smsProcessRepository.CreateWhatsappActivate(Whatsappactivate);
        }
        else
        {
          Whatsappactivate.UpdateWhatsappActivate(jsonStr, _currentUser.EmployeeId, request.IsActive, request.IsDefault);
          var updatedData = await _smsProcessRepository.UpdateWhatsappActivate(Whatsappactivate);
        }

        //remove and add default payment process
        if (request.IsDefault.GetValueOrDefault(false))
        {
          await RemoveDefaultWhatsappActivated(allDefaultWhatsappActivated);
          await MakeDefaultWhatsappActivated(Whatsappactivate);
        }
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = Whatsappactivate?.WhatsAppActivateId, Message = NotificationConstants.Success });
      }
      else
      {
        serviceResult.StatusCode = 400;
        serviceResult.Errors?.Add("SmsLookup", new[] { "Record Not Found" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  #region default
  private async Task MakeDefaultWhatsappActivated(WhatsappActivate item)
  {
    item.MakeDefault(_currentUser.EmployeeId);
    var data = await _smsProcessRepository.UpdateWhatsappActivate(item);
  }
  private async Task RemoveDefaultWhatsappActivated(List<WhatsappActivate>? ppactivates)
  {
    if (ppactivates?.Count > 0)
    {
      foreach (var item in ppactivates)
      {
        item.RemoveDefault(_currentUser.EmployeeId);
        var data = await _smsProcessRepository.UpdateWhatsappActivate(item);
      }
    }
  }
  #endregion
}
