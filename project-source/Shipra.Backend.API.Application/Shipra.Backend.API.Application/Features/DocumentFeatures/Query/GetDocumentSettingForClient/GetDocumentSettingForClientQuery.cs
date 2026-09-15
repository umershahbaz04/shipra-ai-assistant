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
using Shipra.Backend.API.Application.DTOs.MiscUseCase;
using Shipra.Backend.API.Application.Features.MiscFeatures.Query.GetMiscSetting;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DocumentFeatures.Query.GetDocumentSettingForClient;
public class GetDocumentSettingForClientQuery : IRequest<ServiceResultDTO>
{
}
public class GetDocumentSettingForClientQueryHandler : RequestHandlerBase<GetDocumentSettingForClientQuery, ServiceResultDTO>
{
  private readonly IDocumentRepository _documentRepository;

  public GetDocumentSettingForClientQueryHandler(IDocumentRepository documentRepository, IServiceProvider serviceProvider, ILogger<GetDocumentSettingForClientQueryHandler> logger) : base(serviceProvider, logger)
  {
    _documentRepository = documentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDocumentSettingForClientQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var allDocumentTemplate = await _documentRepository.GetAllDocumentTemplate();

      #region mapper  
      List<DocumentTemplateResponseModel> _mapperDocumentTemplateResponseModel = _mapper.Map<List<DocumentTemplateResponseModel>>(allDocumentTemplate);
      #endregion

      var resData = _mapperDocumentTemplateResponseModel.Select(x => new
      {
        x.DocumentTemplateId,
        x.SampleImageUrl,
        x.SamplePdfUrl,
        x.Name
      });



      serviceResult = new ServiceResultDTO(resData!);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
