using System.Dynamic;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.MiscFeatures.Query.GetMiscSetting;
using Shipra.Backend.API.Core.DocumentAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DocumentFeatures.Query.GetValidateDocumentSetting;
public class GetValidateDocumentSettingQuery : IRequest<ServiceResultDTO>
{
}
public class GetValidateDocumentSettingQueryValidator : RequestHandlerBase<GetValidateDocumentSettingQuery, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IDocumentRepository _documentRepository;

  public GetValidateDocumentSettingQueryValidator(IMediator mediator, IDocumentRepository documentRepository, IServiceProvider serviceProvider, ILogger<GetValidateDocumentSettingQueryValidator> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _documentRepository = documentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetValidateDocumentSettingQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
  
      dynamic? oDocumentTemplateConfig = await _documentRepository.GetValidateDocumentSetting((int)EnumDocumentType.AWB, _currentUser.ClientIdStr);

      dynamic? expandoObject = new ExpandoObject();
      expandoObject.DocumentTemplateId = null; 
      if (oDocumentTemplateConfig is null)
      {
        GetDocumentSettingQuery requestDocSetting = new GetDocumentSettingQuery(); 
        var settingData = await _mediator.Send<ServiceResultDTO>(requestDocSetting); 
        expandoObject.result = settingData.Result;
        serviceResult = new ServiceResultDTO(expandoObject);
        //call query
      }
      else
      {
        expandoObject.result = null;
        expandoObject.DocumentTemplateId = oDocumentTemplateConfig.DocumentTemplateId;
        serviceResult = new ServiceResultDTO(expandoObject);
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
