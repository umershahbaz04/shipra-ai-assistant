using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UploadProductFile;

public class UploadProductFileCommandHandler : RequestHandlerBase<UploadProductFileCommand, ServiceResultDTO>
{
  private readonly IS3Service _s3Service;

  public UploadProductFileCommandHandler(IS3Service s3Service, IServiceProvider serviceProvider, ILogger<UploadProductFileCommandHandler> logger) : base(serviceProvider, logger)
  {
    _s3Service = s3Service;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UploadProductFileCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var s3path = ApplicationConstants.GetS3ClientFolderPattern(_currentUser.ClientId!.Value!.ToString(), ApplicationConstants.ClientUploadOrder);

      var requestResponse = await _s3Service.UploadFileAsync(request.File!, s3path);
      var s3responseDto = new S3ResponseDTO();
      s3responseDto.CreateS3ResponseDTO(requestResponse);
      serviceResult = new ServiceResultDTO(s3responseDto!);
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
