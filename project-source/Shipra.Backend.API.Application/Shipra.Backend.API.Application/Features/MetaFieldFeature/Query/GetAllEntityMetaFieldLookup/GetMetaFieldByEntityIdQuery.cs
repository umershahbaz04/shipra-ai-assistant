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
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Application.Features.MetaFieldFeature.Query.GetAllEntityMetaFieldLookup;
public class GetMetaFieldByEntityIdQuery : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
}
public class GetMetaFieldByEntityIdQueryHandler : RequestHandlerBase<GetMetaFieldByEntityIdQuery, ServiceResultDTO>
{
  private readonly IMetaFieldRepository _metaFieldRepository;

  public GetMetaFieldByEntityIdQueryHandler(IMetaFieldRepository metaFieldRepository, IServiceProvider serviceProvider, ILogger<GetMetaFieldByEntityIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _metaFieldRepository = metaFieldRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetMetaFieldByEntityIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _metaFieldRepository.GetMetaFieldsByOrderIdAsync(request.OrderId!);
      var resultList = new List<dynamic>();

      dynamic item = new ExpandoObject();
      if(data != null)
      {
      item.MetaFieldId = data.MetaFieldId;
      item.settingConfig = data?.SettingConfig;
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
