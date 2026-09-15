using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.PaymentProcessAggregate;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.ActivatePaymentProcess;
public class ActivatePaymentProcessCommandHandler : RequestHandlerBase<ActivatePaymentProcessCommand, ServiceResultDTO>
{
  private readonly IPaymentProcessRepository _paymentProcessRepository;

  public ActivatePaymentProcessCommandHandler(IPaymentProcessRepository paymentProcessRepository, IServiceProvider serviceProvider, ILogger<ActivatePaymentProcessCommandHandler> logger) : base(serviceProvider, logger)
  {
    _paymentProcessRepository = paymentProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ActivatePaymentProcessCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var dict = Utils.ConvertKeysToCamelCase(request.InputParameters!);//JsonConvert.DeserializeObject<Dictionary<string, string>>(request.InputParameter!);
      var pplookupId = Utils.GetValueFromDictionryByKey("pplookupId", dict);
      if (!string.IsNullOrEmpty(pplookupId))
      {
        var pplookup = await _paymentProcessRepository.GetPPLookupById(int.Parse(pplookupId));
        #region exist pplookup and config check
        if (pplookup is null)
        {
          throw new EntityNotFoundException("PaymentLookup ", int.Parse(pplookupId));
        }
        if (string.IsNullOrEmpty(pplookup!.InputRequiredConfig!))
        {
          throw new EntityNotFoundException("PaymentProcessConfig ", pplookup!.InputRequiredConfig!);
        }

        #endregion
        var carrierDic = Utils.ConvertKeysToCamelCase(JsonConvert.DeserializeObject<Dictionary<string, string>>(pplookup!.InputRequiredConfig!)!);

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

        var jsonStr = JsonConvert.SerializeObject(carrierDic, Newtonsoft.Json.Formatting.Indented);
        var ppactivate = await _paymentProcessRepository.GetPPActivateByPPLookupId(pplookup.PplookupId, _currentUser.ClientId!);

        //get all activated payment process
        var allDefaultPPActivated = await _paymentProcessRepository.GetAllDefaultPPActivated(_currentUser.ClientId!);
        if (ppactivate == null)
        {
          ppactivate = Ppactivate.CreatePpactivate(pplookup.PplookupId, jsonStr, _currentUser.ClientId, _currentUser.EmployeeId, request.IsDefault);
          ppactivate = await _paymentProcessRepository.CreatePPActivate(ppactivate); 
        }
        else
        {
          ppactivate.UpdatePpactivate(jsonStr, _currentUser.EmployeeId, request.IsActive, request.IsDefault);
          var updatedData = await _paymentProcessRepository.UpdatePPActivate(ppactivate);
        }
        //remove and add default payment process
        if (request.IsDefault.GetValueOrDefault(false))
        {
         await RemoveDefaultPPActivated(allDefaultPPActivated);
         await MakeDefaultPPActivated(ppactivate);
        }
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = ppactivate?.PpactivateId, Message = NotificationConstants.Success });
      }
      else
      {
        serviceResult.StatusCode = 400;
        serviceResult.Errors?.Add("PPLookupNotFound", new[] { "Record Not Found" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task MakeDefaultPPActivated(Ppactivate item)
  {
    item.MakeDefault(_currentUser.EmployeeId);
    var data = await _paymentProcessRepository.UpdatePPActivate(item);
  }

  private async Task RemoveDefaultPPActivated(List<Ppactivate>? ppactivates)
  {
    if(ppactivates?.Count > 0)
    {
      foreach (var item in ppactivates)
      {
        item.RemoveDefault(_currentUser.EmployeeId);
        var data = await _paymentProcessRepository.UpdatePPActivate(item);
      }
    }
  }
}

