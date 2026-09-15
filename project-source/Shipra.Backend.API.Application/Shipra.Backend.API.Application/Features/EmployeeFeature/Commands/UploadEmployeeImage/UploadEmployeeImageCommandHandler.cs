using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.UploadEmployeeImage;
public class UploadEmployeeImageCommandHandler : RequestHandlerBase<UploadEmployeeImageCommand, ServiceResultDTO>
{
  private readonly IS3Service _s3Service;
  private readonly IEmployeeRepository _employeeRepository;

  public UploadEmployeeImageCommandHandler(IS3Service s3Service, IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<UploadEmployeeImageCommandHandler> logger) : base(serviceProvider, logger)
  {
    _s3Service = s3Service;
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UploadEmployeeImageCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filePath = ApplicationConstants.EmployeeImagePath + DateTime.Today.ToString("dd-MM-yyyy") + "/";
      var requestResponse = await _s3Service.UploadFileAsync(request.File!, filePath);
      var s3ResponseDTO = new S3ResponseDTO();
      s3ResponseDTO.CreateS3ResponseDTO(requestResponse);

      serviceResult = new ServiceResultDTO(s3ResponseDTO!);
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
