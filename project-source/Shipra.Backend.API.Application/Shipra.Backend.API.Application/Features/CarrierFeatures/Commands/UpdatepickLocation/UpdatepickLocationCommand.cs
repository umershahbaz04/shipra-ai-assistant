using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.CarrierAggregate;
 
namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.UpdatepickLocation;

public class UpdatepickLocationCommand : IRequest<ServiceResultDTO>
{
  public int ActiveCarrierPickupLocationId { get; set; }
 
  public string? LocationName { get; set; }
  public string? CustomerServiceNo { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public int? CountryId { get; set; }
  public int? CityId { get; set; }
  public int? AreaId { get; set; }
  public string? Phone { get; set; }
  public string? StreetAddress { get; set; }
  public string? StreetAddress2 { get; set; }
  public string? HouseNo { get; set; }
  public string? BuildingName { get; set; }
  public string? Landmark { get; set; }
  public string? Zip { get; set; }

}

public class UpdatepickLocationCommandHandler : RequestHandlerBase<UpdatepickLocationCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public UpdatepickLocationCommandHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider,ILogger<UpdatepickLocationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdatepickLocationCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
  
      var pickupLocation = await _carrierRepository.GetActiveCarrierLocationbyId(request.ActiveCarrierPickupLocationId, _currentUser.ClientId!);
      if (pickupLocation == null)
      {
        throw new EntityNotFoundException("ActiveCarrierPickupLocation", request.ActiveCarrierPickupLocationId);
      }
      var code = ActiveCarrierPickupLocation.GetCode(request.LocationName!);
      pickupLocation.UpdatepickLocation(request.LocationName,
        request.Phone,
        request.CountryId,
        request.CityId,
        request.AreaId,

        request.StreetAddress,
        request.StreetAddress2,
        request.Longitude,
        request.Latitude,
        request.CustomerServiceNo,
        request.HouseNo,
        request.BuildingName,
        request.Landmark,
        request.Zip,
        code,
        "",
        ""
      );

      var updated = await _carrierRepository.UpdatepickLocation(pickupLocation);
      if (updated != null)
      {
        var result = new BaseResponseDto
        {
          Data = updated.ActiveCarrierPickupLocationId,
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

public class UpdatepickLocationCommandValidator : AbstractValidator<UpdatepickLocationCommand>
{
  public UpdatepickLocationCommandValidator()
  {
     
  }
}
