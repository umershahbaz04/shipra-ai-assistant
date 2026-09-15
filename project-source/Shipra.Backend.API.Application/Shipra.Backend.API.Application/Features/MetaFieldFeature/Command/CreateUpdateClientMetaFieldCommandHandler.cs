using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ExpenseFeatures.Command.CreateExpenseCategory;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.MetaFieldAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;
public class CreateUpdateClientMetaFieldCommandHandler : RequestHandlerBase<CreateUpdateClientMetaFieldCommand, ServiceResultDTO>
{
  private readonly IMetaFieldRepository _metaFieldRepository;
  public CreateUpdateClientMetaFieldCommandHandler(IMetaFieldRepository metaFieldRepository, IServiceProvider serviceProvider, ILogger<CreateUpdateClientMetaFieldCommandHandler> logger) : base(serviceProvider, logger)
  {
    _metaFieldRepository = metaFieldRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(CreateUpdateClientMetaFieldCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<int> updatedIds = new();
      foreach (var item in request.ClientMetaFields!)
      {
        var settingConfigJson = JsonConvert.SerializeObject(item.settingConfig);
        var ClientMetaFieldData = await _metaFieldRepository.GetClientMetaDataById(item.clientMetaFieldId!, _currentUser.ClientId!);
        if (item.clientMetaFieldId is null)
        {
          var Clientmetadata = await _metaFieldRepository.CreateClientMetaField(ClientMetaField.CreateClientMetaField(_currentUser.ClientId!, item.entityMetaFieldId!, settingConfigJson));
          updatedIds.Add(Clientmetadata.ClientMetaFieldId);
        }
        else
        {
          ClientMetaFieldData!.UpdateclientMetaData(settingConfigJson, item.entityMetaFieldId!);
          var updatedData = await _metaFieldRepository.UpdateClientMetaData(ClientMetaFieldData);
          updatedIds.Add(updatedData.ClientMetaFieldId);
        }
      }
      serviceResult = new ServiceResultDTO();
      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = updatedIds, Message = NotificationConstants.Success });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

 
