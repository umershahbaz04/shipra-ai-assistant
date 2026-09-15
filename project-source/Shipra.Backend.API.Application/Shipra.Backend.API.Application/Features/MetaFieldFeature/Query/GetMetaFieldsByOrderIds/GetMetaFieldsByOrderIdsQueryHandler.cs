using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Features.MetaFieldFeature.Query.GetMetaFieldsByOrderIds;

public class GetMetaFieldsByOrderIdsQueryHandler : RequestHandlerBase<GetMetaFieldsByOrderIdsQuery, ServiceResultDTO>
{
  private readonly IMetaFieldRepository _metaFieldRepository;

  public GetMetaFieldsByOrderIdsQueryHandler(IMetaFieldRepository metaFieldRepository, IServiceProvider serviceProvider, ILogger<GetMetaFieldsByOrderIdsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _metaFieldRepository = metaFieldRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetMetaFieldsByOrderIdsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      if (request.OrderIds == null || request.OrderIds.Count == 0)
      {
         return new ServiceResultDTO(new List<dynamic>());
      }

      var data = await _metaFieldRepository.GetMetaFieldsByOrderIdsAsync(request.OrderIds);
      var resultList = new List<dynamic>();

      foreach(var meta in data)
      {
         dynamic item = new ExpandoObject();
         item.MetaFieldId = meta.MetaFieldId;
         item.OrderId = meta.EntityId;
         item.settingConfig = meta.SettingConfig;
         resultList.Add(item);
      }

      serviceResult = new ServiceResultDTO(resultList);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
