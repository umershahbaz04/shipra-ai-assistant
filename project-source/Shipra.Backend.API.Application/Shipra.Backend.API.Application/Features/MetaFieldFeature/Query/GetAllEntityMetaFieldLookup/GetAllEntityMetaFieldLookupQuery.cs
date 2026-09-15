using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Query;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Application.Features.MetaFieldFeature.Query.GetAllEntityMetaFieldLookup;
public class GetAllEntityMetaFieldLookupQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllEntityMetaFieldLookupQueryHandler : RequestHandlerBase<GetAllEntityMetaFieldLookupQuery, ServiceResultDTO>
{
  private readonly IMetaFieldRepository _metaFieldRepository;

  public GetAllEntityMetaFieldLookupQueryHandler(IMetaFieldRepository metaFieldRepository, IServiceProvider serviceProvider, ILogger<GetAllEntityMetaFieldLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _metaFieldRepository = metaFieldRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllEntityMetaFieldLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _metaFieldRepository.GetMetaFields();
      var clientMetaDataList = await _metaFieldRepository.GetClientMetaDataByClientId(_currentUser.ClientId!);
      var resultList = new List<dynamic>();
      foreach (var field in data)
      {
        var matchedMeta = clientMetaDataList.FirstOrDefault(x => x.EntityMetaFieldId == field.EntityMetaFieldId);

        dynamic item = new ExpandoObject();
        item.entityMetaFieldId = field.EntityMetaFieldId;
        item.typeName = field.TypeName;
        item.settingConfig = matchedMeta?.SettingConfig ?? field.SettingConfig;
        item.clientMetaFieldId = matchedMeta?.ClientMetaFieldId;

        resultList.Add(item);

      }

      serviceResult = new ServiceResultDTO(resultList!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
