using System.Dynamic;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateImageGallery;
public class CreateImageGalleryCommand : IRequest<ServiceResultDTO>
{
  public List<CreateImageGalleryRequestDto>? List { get; set; }
}
public class CreateImageGalleryCommandHandler : RequestHandlerBase<CreateImageGalleryCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  private readonly IS3Service _s3Service;

  public CreateImageGalleryCommandHandler(IProductRepository productRepository, IS3Service s3Service, IServiceProvider serviceProvider, ILogger<CreateImageGalleryCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
    _s3Service = s3Service;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateImageGalleryCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filePath = "Product/ImageGallery" + DateTime.Today.ToString("dd-MM-yyyy") + "/"; ;
      var s3ResponseList = new List<dynamic>();
      foreach (var item in request.List!)
      {
        var requestResponse = await _s3Service.UploadFileAsync(item!.File!, filePath);
        //var s3ResponseDTO = new S3ResponseDTO();
        //s3ResponseDTO.CreateS3ResponseDTO(requestResponse);
      
        int mediaTypeId = UtilityHelper.GetMediaTypeId(item.File!);
        ImageGallery imageGallery = ImageGallery.Create(_currentUser.ClientId, requestResponse.Url, mediaTypeId, item.File!.FileName.Replace(" ", ""), item.Description,_currentUser.EmployeeId!);

        await _productRepository.CreateImageGallery(imageGallery);

        dynamic s3ResponseDTO = new ExpandoObject(); 
        s3ResponseDTO.imageUrl = requestResponse.Url;
        s3ResponseDTO.message = requestResponse.Message;
        s3ResponseDTO.imageGalleryId = imageGallery.ImageGalleryId;
        s3ResponseList.Add(s3ResponseDTO);
      } 
      serviceResult = new ServiceResultDTO(s3ResponseList!);
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
public class CreateImageGalleryCommandValidator : AbstractValidator<CreateImageGalleryCommand>
{
  public CreateImageGalleryCommandValidator()
  {
    RuleFor(x=>x.List).NotEmpty().WithMessage("Image gallery list cannot be empty.");
  } 
}
