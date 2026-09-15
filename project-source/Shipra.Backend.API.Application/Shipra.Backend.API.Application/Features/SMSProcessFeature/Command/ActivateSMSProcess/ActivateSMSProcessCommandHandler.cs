using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.PaymentProcessAggregate;
using Shipra.Backend.API.Core.SMSProcessAggregate;

namespace Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.ActivateSMSProcess;
public class ActivateSMSProcessCommandHandler : RequestHandlerBase<ActivateSMSProcessCommand, ServiceResultDTO>
{
  private readonly ISMSProcessRepository _smsProcessRepository;

  public ActivateSMSProcessCommandHandler(ISMSProcessRepository smsProcessRepository, IServiceProvider serviceProvider, ILogger<ActivateSMSProcessCommandHandler> logger) : base(serviceProvider, logger)
  {
    _smsProcessRepository = smsProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ActivateSMSProcessCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var dict = Utils.ConvertKeysToCamelCase(request.InputParameters!);
      var sMSlookupId = Utils.GetValueFromDictionryByKey("sMSlookupId", dict);
      if (!string.IsNullOrEmpty(sMSlookupId))
      {
        var smslookup = await _smsProcessRepository.GetSMSLookupById(int.Parse(sMSlookupId));
        #region exist SMSlookup and config check
        if (smslookup is null)
        {
          throw new EntityNotFoundException("SMSLookup ", int.Parse(sMSlookupId));
        }
        if (string.IsNullOrEmpty(smslookup!.Config!))
        {
          throw new EntityNotFoundException("SMSProcessConfig ", smslookup!.Config!);
        }

        #endregion
        var carrierDic = Utils.ConvertKeysToCamelCase(JsonConvert.DeserializeObject<Dictionary<string, string>>(smslookup!.Config!)!);

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
        var smsactivate = await _smsProcessRepository.GetSMSActivateBySMSLookupId(smslookup.SMSLookupId, _currentUser.ClientId!);

        //get all activated payment process
        var allDefaultSmsActivated = await _smsProcessRepository.GetAllDefaultSmsActivated(_currentUser.ClientId!);

        #region get all client sms if result 0 then make it defualt
        var allClientMessages = await _smsProcessRepository.GetAllSmsActivated(_currentUser.ClientId!);
        if (allClientMessages!.Count == 0 || !allClientMessages.Any(x => x.IsDefault == true))
        {
          request.IsDefault = true;
        } 
        #endregion

        if (smsactivate == null)
        {
          smsactivate = SMSActivate.CreateSMSASctivate(smslookup.SMSLookupId, jsonStr, _currentUser.ClientId, _currentUser.EmployeeId,request.IsDefault);
          smsactivate = await _smsProcessRepository.CreateSMSActivate(smsactivate);
        }
        else
        {
          smsactivate.UpdateSMSActivate(jsonStr, _currentUser.EmployeeId,request.IsActive,request.IsDefault);
          var updatedData = await _smsProcessRepository.UpdateSMSActivate(smsactivate);
        }

        //remove and add default payment process
        if (request.IsDefault.GetValueOrDefault(false))
        {
          await RemoveDefaultSmsActivated(allDefaultSmsActivated);
          await MakeDefaultSmsActivated(smsactivate);
        }
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = smsactivate?.SMSActivateId, Message = NotificationConstants.Success });
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
  private async Task MakeDefaultSmsActivated(SMSActivate item)
  {
    item.MakeDefault(_currentUser.EmployeeId);
    var data = await _smsProcessRepository.UpdateSMSActivate(item);
  }

  private async Task RemoveDefaultSmsActivated(List<SMSActivate>? ppactivates)
  {
    if (ppactivates?.Count > 0)
    {
      foreach (var item in ppactivates)
      {
        item.RemoveDefault(_currentUser.EmployeeId);
        var data = await _smsProcessRepository.UpdateSMSActivate(item);
      }
    }
  }
}

