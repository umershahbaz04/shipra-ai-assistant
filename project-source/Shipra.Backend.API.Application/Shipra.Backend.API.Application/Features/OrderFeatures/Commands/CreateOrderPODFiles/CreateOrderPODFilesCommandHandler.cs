using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderPODFiles;
public class CreateOrderPODFilesCommandHandler : RequestHandlerBase<CreateOrderPODFilesCommand, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IS3Service _s3Service;
  private readonly IOrderRepository _orderRepository;

  public CreateOrderPODFilesCommandHandler(IWebHostEnvironment webHostEnvironment, IS3Service s3Service, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CreateOrderPODFilesCommandHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _s3Service = s3Service;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateOrderPODFilesCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    BaseResponseDto baseResponse = new BaseResponseDto();
    List<string> successList = new List<string>();
    try
    {
      string fileUploadsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
      if (!Directory.Exists(fileUploadsFolder))
      { //check if the folder exists;
        Directory.CreateDirectory(fileUploadsFolder);
      }
      foreach (var file in request.FilesList!)
      {
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        string filePath = System.IO.Path.Combine(fileUploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          file?.CopyTo(fileStream);
        }
        var s3path = ApplicationConstants.GetS3ClientFolderPattern(_currentUser.ClientId!.Value!.ToString(), ApplicationConstants.ClientUploadOrderPOD);
        var s3Response = await _s3Service.UploadFileWithExpandoresultAsync(file!, s3path);

        await _orderRepository.CreateOrderPODFiles(OrderPODFile.CreateOrderPODFiles(new OrderId(new Guid(request.OrderId!)), request.Comment!, s3Response.Url, Path.GetExtension(file!.FileName), _currentUser.EmployeeId!));
        successList.Add(s3Response.Url);
        if (System.IO.File.Exists(filePath))
        {
          System.IO.File.Delete(filePath);
        }
      }
      if (successList is not null && successList.Count > 0)
      {
        baseResponse = new BaseResponseDto()
        {
          Data = successList,
          Message = "Selected files uploaded successfully."
        };
        serviceResult = new ServiceResultDTO(baseResponse);
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
