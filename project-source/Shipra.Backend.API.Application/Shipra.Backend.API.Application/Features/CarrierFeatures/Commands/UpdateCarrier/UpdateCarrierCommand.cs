using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.UpdateCarrier;

public class UpdateCarrierCommand : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
  public string? Name { get; set; }
  public string? CarrierImage { get; set; }
  public string? CarrierWebsite { get; set; }
  public string? Config { get; set; }
  public string? InputRequiredConfig { get; set; }
  public int? CountryId { get; set; }
}
public class UpdateCarrierCommandHandler : RequestHandlerBase<UpdateCarrierCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public UpdateCarrierCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<UpdateCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateCarrierCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var carrier = await _carrierRepository.GetCarrierById(request.CarrierId);
      if (carrier == null)
      {
        throw new EntityNotFoundException("Carrier", request.CarrierId);
      }
      carrier?.UpdateCarrier(request.Name, request.CarrierImage, request.CarrierWebsite, request.Config, request.InputRequiredConfig, request.CountryId, _currentUser.EmployeeId!);

      var oCarrier = await _carrierRepository.UpdateCarrier(carrier!);
      if (oCarrier is not null)
      {
        var result = new BaseResponseDto()
        {
          Data = oCarrier.CarrierId,
          Message = NotificationConstants.Success
        };
        serviceResult = new ServiceResultDTO(result);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        return serviceResult;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.Error));
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class UpdateCarrierCommandValidator : AbstractValidator<UpdateCarrierCommand>
{
  public UpdateCarrierCommandValidator()
  {
    RuleFor(v => v.CarrierId).NotNull().NotEmpty().GreaterThan(0);

    RuleFor(v => v.Name).NotNull().NotEmpty();
    RuleFor(v => v.CarrierWebsite).NotNull().NotEmpty();
    RuleFor(v => v.CarrierImage).NotNull().NotEmpty();
    RuleFor(v => v.CountryId).NotNull().NotEmpty().GreaterThan(0);
  }
}
