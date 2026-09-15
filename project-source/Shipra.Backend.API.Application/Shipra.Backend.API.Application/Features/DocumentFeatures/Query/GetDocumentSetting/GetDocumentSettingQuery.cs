using System.Dynamic;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.MiscUseCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.MiscFeatures.Query.GetMiscSetting;
public class GetDocumentSettingQuery : IRequest<ServiceResultDTO>
{
}
public class GetDocumentSettingQueryHandler : RequestHandlerBase<GetDocumentSettingQuery, ServiceResultDTO>
{
  private readonly IDocumentRepository _documentRepository;

  public GetDocumentSettingQueryHandler(IDocumentRepository documentRepository, IServiceProvider serviceProvider, ILogger<GetDocumentSettingQueryHandler> logger) : base(serviceProvider, logger)
  {
    _documentRepository = documentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDocumentSettingQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = _currentUser.ClientIdStr;
      var allDocumentTypes = await _documentRepository.GetAllDocumentType();
      var allDocumentSize = await _documentRepository.GetAllDocumentSize();
      var allDocumentTemplate = await _documentRepository.GetAllDocumentTemplate();
      var allDocumentTemplateConfigByClient = await _documentRepository.GetAllDocumentTemplateConfigByClient(_currentUser.ClientId);

      #region mapper 
      List<DocumentSizeResponseModel> _mapperDocumentSizeResponseModel = _mapper.Map<List<DocumentSizeResponseModel>>(allDocumentSize);
      List<DocumentTemplateResponseModel> _mapperDocumentTemplateResponseModel = _mapper.Map<List<DocumentTemplateResponseModel>>(allDocumentTemplate);
      #endregion

      List<dynamic> responseList = new List<dynamic>();
      foreach (var docType in allDocumentTypes)
      {
        dynamic response = new ExpandoObject();
        response.value = docType.Value;
        response.documentTypeId = docType.DocumentTypeId;
        response.haveSize = docType.HaveSize;
        response.TemplateName = string.Empty;

        var documentTemplates = allDocumentTemplate.Where(x => x.DocumentTypeId == docType.DocumentTypeId).ToList();

        dynamic? selectedObj = new ExpandoObject();
        response.documentTemplateConfigId = string.Empty;
        response.IsAskEveryTime = false;

        dynamic? responseSelected = new ExpandoObject();
        responseSelected.DocumentTypeId = 0;
        responseSelected.DocumentSizeId = 0;
        responseSelected.DocumentTemplateId = 0;
        responseSelected.TemplateName = string.Empty;
        responseSelected.IsAskEveryTime = false;

        foreach (var item in documentTemplates)
        {
          var clientSelected = allDocumentTemplateConfigByClient.FirstOrDefault(x => x.DocumentTemplateId == item.DocumentTemplateId);

          if (clientSelected != null)
          {
            response.documentTemplateConfigId = clientSelected!.DocumentTemplateConfigId!.Value;
            response.IsAskEveryTime = clientSelected.IsAskEveryTime.GetValueOrDefault();
            response.TemplateName = clientSelected!.TemplateName!;
            selectedObj = allDocumentTemplate.FirstOrDefault(x => x.DocumentTemplateId == clientSelected.DocumentTemplateId && x.DocumentTypeId == docType.DocumentTypeId);
            if (selectedObj != null)
            {
              responseSelected.DocumentTypeId = docType.DocumentTypeId;
              responseSelected.DocumentSizeId = selectedObj.DocumentSizeId;
              responseSelected.DocumentTemplateId = selectedObj.DocumentTemplateId;
              responseSelected.documentTemplateConfigId = clientSelected.DocumentTemplateConfigId.Value;
              responseSelected.IsAskEveryTime = clientSelected.IsAskEveryTime.GetValueOrDefault();
              responseSelected.TemplateName = clientSelected.TemplateName;
              responseSelected.IsAskEveryTime = clientSelected.IsAskEveryTime.GetValueOrDefault();
            }
          } 
        }

        if (docType.HaveSize.GetValueOrDefault())
        {
          if (selectedObj != null)
          {
            SetIsSelectedSizeFlag(_mapperDocumentSizeResponseModel!, responseSelected.DocumentSizeId);
          }
          response.documentSize = _mapperDocumentSizeResponseModel;
        }
        if (selectedObj != null)
        {
          SetIsSelectedTemplateFlag(_mapperDocumentTemplateResponseModel!, responseSelected.DocumentTemplateId); 
        }
        response.documentTemplate = _mapperDocumentTemplateResponseModel;

        responseList.Add(response);
      }

      serviceResult = new ServiceResultDTO(responseList!);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private void SetIsSelectedTemplateFlag(List<DocumentTemplateResponseModel> documentTemplateResponseModels, int documentTemplateId)
  {
    foreach (var item in documentTemplateResponseModels!)
    {
      item.IsSelected = (item.DocumentTemplateId == documentTemplateId);
    }
  }

  private void SetIsSelectedSizeFlag(List<DocumentSizeResponseModel> documentSizes, int selectedDocumentSizeId)
  {
    foreach (var size in documentSizes!)
    {
      size.IsSelected = (size.DocumentSizeId == selectedDocumentSizeId);
    }
  }
}
