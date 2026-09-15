using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.MetaFieldAggregate;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;

public class UpdateMetaFieldForOrdersCommandHandler : RequestHandlerBase<UpdateMetaFieldForOrdersCommand, ServiceResultDTO>
{
  private readonly IMetaFieldRepository _metaFieldRepository;

  public UpdateMetaFieldForOrdersCommandHandler(IMetaFieldRepository metaFieldRepository, IServiceProvider serviceProvider, ILogger<UpdateMetaFieldForOrdersCommandHandler> logger) : base(serviceProvider, logger)
  {
    _metaFieldRepository = metaFieldRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateMetaFieldForOrdersCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      if (request.OrderIds == null || request.OrderIds.Count == 0)
      {
         return new ServiceResultDTO(new BaseResponseDto { Message = NotificationConstants.Success });
      }

      foreach (var orderId in request.OrderIds)
      {
        var metaField = await _metaFieldRepository.GetMetaFieldDataByOrderId(orderId);
        if (metaField == null)
        {
          var newMetaField = MetaField.CreateMetaField(request.SettingConfig!, orderId, _currentUser.ClientId!);
          await _metaFieldRepository.CreateMetaFields(newMetaField);
        }
        else
        {
          metaField.UpdateMetaField(request.SettingConfig!);
          await _metaFieldRepository.UpdateMetaFieldData(metaField);
        }
      }

      serviceResult = new ServiceResultDTO(new BaseResponseDto { Message = NotificationConstants.Success });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
