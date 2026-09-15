using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.CreateReturnOrder;
public class CreateReturnOrderCommand : IRequest<ServiceResultDTO>
{
  public string? ClientId { get; set; }
  public string? OrderId { get; set; }
  public int ClientReturnReasonId { get; set; }
  public string? ReturnComment { get; set; } 
  public int OrderTypeId { get; set; }
  //public decimal? ReturnCharges { get; set; }
  public List<ReturnProductRequestModel>? ReturnProducts { get; set; }
  public IFormFile? File { get; set; } // This will be the image file uploaded
}
public class ReturnProductRequestModel
{
  public string? ProductId { get; set; }
  public int? ProductStockId { get; set; }
  public decimal? ItemValue { get; set; }
}
public class CreateReturnOrderCommandHandler : RequestHandlerBase<CreateReturnOrderCommand, ServiceResultDTO>
{
  private readonly IS3Service _s3Service;
  private readonly IReturnRepository _returnRepository;

  public CreateReturnOrderCommandHandler(IS3Service s3Service, IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<CreateReturnOrderCommandHandler> logger) : base(serviceProvider, logger)
  {
    _s3Service = s3Service;
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateReturnOrderCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      request.ClientId = !string.IsNullOrEmpty(request.ClientId) ? request.ClientId : _currentUser.ClientIdStr;

      request.ClientId = request.ClientId!.Trim();
      request.OrderId = request.OrderId!.Trim();
      var clientId = new ClientId(new Guid(request.ClientId!));

      var order = await _returnRepository.GetOrderById(new OrderId(new Guid(request.OrderId!)), clientId!);
      if (order is null)
      {
        throw new EntityNotFoundException("Order ", request.OrderId!);
      }
      var objReturnRes = await _returnRepository.GetReturnReportDataByOrderIdForTracking(order.OrderNo, request.ClientId);
      if (!objReturnRes.IsReturnExist.GetValueOrDefault())
      {
        // we will enter manually or define rules after
        decimal? returnCharges = 0;
        string? fileUrl = string.Empty;
        var s3path = ApplicationConstants.GetS3ClientFolderPattern(request.ClientId, ApplicationConstants.ClientUploadReturnOrder);
        if (request.File != null)
        {
          var requestResponse = await _s3Service.UploadFileAsync(request.File!, s3path);
          fileUrl = requestResponse.Url;
        }
        var refundAmount = order.Amount.GetValueOrDefault() - returnCharges.GetValueOrDefault();
        var returnObj = Return.Create(clientId, order.OrderId!, request.ClientReturnReasonId, request.ReturnComment!, request.OrderTypeId, returnCharges, refundAmount, fileUrl);
        bool? isCreated = await _returnRepository.CreateReturn(returnObj, request.ClientId);
        if (isCreated.GetValueOrDefault())
        { 
          var isAd = await _returnRepository.CreateReturnTrackingHistory(ReturnTrackingHistory.Create(returnObj.ReturnId!,(int)EnumReturnStatus.Created,""), request.ClientId);
          if (request.OrderTypeId == (int)EnumOrderType.FullFilable && request.ReturnProducts?.Count() > 0)
          {
            foreach (var item in request.ReturnProducts)
            {
              ReturnProduct returnProduct = ReturnProduct.Create(returnObj.ReturnId, new ProductId(new Guid(item!.ProductId!)), item.ProductStockId, item.ItemValue);
              bool isAdded = await _returnRepository.CreateReturnProduct(returnProduct, request.ClientId);
            }
          }
          serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = null, Message = "Return created successfully" });
        }
      }
      else
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = null, Message = $"Return already created against OrderNo:{order.OrderNo}" }, false);
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
public class CreateReturnOrderCommandValidator : AbstractValidator<CreateReturnOrderCommand>
{
  public CreateReturnOrderCommandValidator()
  {
    RuleFor(v => v.OrderId).NotNull().NotEmpty()
            .Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(v => v.OrderTypeId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.ClientReturnReasonId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.ReturnComment).NotNull().NotEmpty(); 

    When(v => v.OrderTypeId == (int)EnumOrderType.FullFilable, () =>
    {
      RuleFor(v => v.ReturnProducts).NotNull().WithMessage("Return Product is required for fullfillable orders.");
      // Ensure that ProductId validation only happens if ReturnProduct is not null
      When(v => v.ReturnProducts != null, () =>
      {
        RuleForEach(model => model.ReturnProducts).SetValidator(model => new CreateProductValidator());
      });
    });
  }
}
public class CreateProductValidator : AbstractValidator<ReturnProductRequestModel>
{
  public CreateProductValidator()
  {
    RuleFor(v => v.ProductId).NotNull().NotEmpty()
           .Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(v => v.ProductStockId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.ItemValue).NotNull().NotEmpty();
  }
}
