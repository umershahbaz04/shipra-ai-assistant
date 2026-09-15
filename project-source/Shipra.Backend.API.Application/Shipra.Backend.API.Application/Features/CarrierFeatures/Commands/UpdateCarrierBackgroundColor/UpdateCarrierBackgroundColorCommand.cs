using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.ShipmentUseCase;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.UpdateShipmentGridColumn;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.UpdateCarrierBackgroundColor;
public class UpdateCarrierBackgroundColorCommand : IRequest<ServiceResultDTO>
{
  public List<UpdateCarrierBackgroundColorRequestModel>? list { get; set; }
}
public class UpdateCarrierBackgroundColorCommandHandler : RequestHandlerBase<UpdateCarrierBackgroundColorCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public UpdateCarrierBackgroundColorCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<UpdateCarrierBackgroundColorCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateCarrierBackgroundColorCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      foreach (var item in request.list!)
      {
        var carrier = await _carrierRepository.GetCarrierById(item.CarrierId);
        if (carrier is not null)
        {
          carrier?.UpdateCarrierColors(item.BackgroundColor, item.BorderColor);
          var oCarrier = await _carrierRepository.UpdateCarrier(carrier!);
        } 
      }
      var result = new BaseResponseDto()
      {
        Data = "",
        Message = NotificationConstants.Success
      };
      serviceResult = new ServiceResultDTO(result);
      serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class UpdateCarrierBackgroundColorCommandValidator : AbstractValidator<UpdateCarrierBackgroundColorCommand>
{
  public UpdateCarrierBackgroundColorCommandValidator()
  {
    RuleFor(x => x.list).Must(x => x != null).WithMessage("OrderItems list must contain at least one item.");
    RuleForEach(x => x.list).SetValidator(x => new UpdateCarrierBackgroundColorRequestModelValidator());
  }
}
public class UpdateCarrierBackgroundColorRequestModelValidator : AbstractValidator<UpdateCarrierBackgroundColorRequestModel>
{
  public UpdateCarrierBackgroundColorRequestModelValidator()
  {
    RuleFor(v => v.CarrierId).NotNull().GreaterThan(0);
    RuleFor(v => v.BackgroundColor).NotNull().NotEmpty();
    RuleFor(v => v.BorderColor).NotNull().NotEmpty();
  }
}
