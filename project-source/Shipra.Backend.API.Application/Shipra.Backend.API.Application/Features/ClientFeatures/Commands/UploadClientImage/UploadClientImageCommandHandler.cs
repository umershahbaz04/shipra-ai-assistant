using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UploadClientImage;
public class UploadClientImageCommandHandler : RequestHandlerBase<UploadClientImageCommand, ServiceResultDTOWithTypeModel<S3ResponseDTO>>
{
  private readonly IS3Service _s3Service;
  public UploadClientImageCommandHandler(IS3Service s3Service, IServiceProvider serviceProvider, ILogger<UploadClientImageCommandHandler> logger) : base(serviceProvider, logger)
  {
    _s3Service = s3Service;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<S3ResponseDTO>> HandleRequest(UploadClientImageCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<S3ResponseDTO> serviceResult = new ServiceResultDTOWithTypeModel<S3ResponseDTO>();
    try
    {
      var filePath = ApplicationConstants.ClientProfileImagePath + DateTime.Today.ToString("dd-MM-yyyy") + "/";
      var requestResponse = await _s3Service.UploadFileAsync(request.File!, filePath);
      var s3responseDto = new S3ResponseDTO();
      s3responseDto.CreateS3ResponseDTO(requestResponse);
      serviceResult = new ServiceResultDTOWithTypeModel<S3ResponseDTO>(s3responseDto!);
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
