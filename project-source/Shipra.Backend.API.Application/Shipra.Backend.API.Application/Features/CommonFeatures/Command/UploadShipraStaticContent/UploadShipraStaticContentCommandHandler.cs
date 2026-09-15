using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Command.UploadShipraStaticContent;
public class UploadShipraStaticContentCommandHandler : RequestHandlerBase<UploadShipraStaticContentCommand, ServiceResultDTOWithTypeModel<S3ResponseDTO>>
{
  private readonly IS3Service _s3Service;
  public UploadShipraStaticContentCommandHandler(IS3Service s3Service, IServiceProvider serviceProvider, ILogger<UploadShipraStaticContentCommandHandler> logger) : base(serviceProvider, logger)
  {
    _s3Service = s3Service;
  }
  protected override async Task<ServiceResultDTOWithTypeModel<S3ResponseDTO>> HandleRequest(UploadShipraStaticContentCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<S3ResponseDTO> serviceResult = new ServiceResultDTOWithTypeModel<S3ResponseDTO>();
    try
    {
      var folderName = "";
      var filePath = "";
      var requestPath = !string.IsNullOrWhiteSpace(request.Path) ? request.Path?.Replace(" ", "").ToLower().Trim() : "";
      if (request.SCFolderLookupId == (int)EnumSCFolderLookup.Carrier)
      {
        folderName = EnumSCFolderLookupHelper.GetEnumString(EnumSCFolderLookup.Carrier);
      }
      else if (request.SCFolderLookupId == (int)EnumSCFolderLookup.Order)
      {
        folderName = EnumSCFolderLookupHelper.GetEnumString(EnumSCFolderLookup.Order);
      } 
      else if (request.SCFolderLookupId == (int)EnumSCFolderLookup.Others)
      {
        folderName = EnumSCFolderLookupHelper.GetEnumString(EnumSCFolderLookup.Others);
      }
      filePath = "Shipra/" + folderName + "/" + requestPath + "/" + DateTime.Today.ToString("dd-MM-yyyy") + "/";
      var requestResponse = await _s3Service.UploadFileAsync(request.File!, filePath);
      var s3ResponseDTO = new S3ResponseDTO();
      s3ResponseDTO.CreateS3ResponseDTO(requestResponse);

      serviceResult = new ServiceResultDTOWithTypeModel<S3ResponseDTO>(s3ResponseDTO!);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
