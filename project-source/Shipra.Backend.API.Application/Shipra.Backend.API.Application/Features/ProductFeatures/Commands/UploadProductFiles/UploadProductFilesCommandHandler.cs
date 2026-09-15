using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UploadProductFiles;

public class UploadProductFilesCommandHandler : RequestHandlerBase<UploadProductFilesCommand, ServiceResultDTOWithTypeModel<List<S3ResponseDTO>>>
{
  private readonly IS3Service _s3Service;

  public UploadProductFilesCommandHandler(IS3Service s3Service, IServiceProvider serviceProvider, ILogger<UploadProductFilesCommandHandler> logger) : base(serviceProvider, logger)
  {
    _s3Service = s3Service;
  }

  public List<IFormFile>? Files { get; set; }

  protected override async Task<ServiceResultDTOWithTypeModel<List<S3ResponseDTO>>> HandleRequest(UploadProductFilesCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<List<S3ResponseDTO>> serviceResult = new ServiceResultDTOWithTypeModel<List<S3ResponseDTO>>();
    try
    {
      var filePath = "Product/ProductFiles" + DateTime.Today.ToString("dd-MM-yyyy") + "/"; ;
      var s3ResponseList = new List<S3ResponseDTO>();
      foreach (var item in request.Files!)
      {
        var requestResponse = await _s3Service.UploadFileAsync(item!, filePath);
        var s3ResponseDTO = new S3ResponseDTO();
        s3ResponseDTO.CreateS3ResponseDTO(requestResponse);

        s3ResponseList.Add(s3ResponseDTO);
      }

      serviceResult = new ServiceResultDTOWithTypeModel<List<S3ResponseDTO>>(s3ResponseList!);
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
