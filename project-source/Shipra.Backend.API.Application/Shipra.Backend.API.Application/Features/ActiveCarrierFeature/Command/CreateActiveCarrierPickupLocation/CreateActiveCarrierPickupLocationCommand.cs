 
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
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrierPickupLocation;

public class CreateActiveCarrierPickupLocationCommand : AddressRequestDTO, IRequest<ServiceResultDTO>
{
  public int ActiveCarrierPickupLocationId { get; set; }
  public string? CustomerServiceNo { get; set; }
  public string? EntityAddressDataJson { get; set; }
  public int? CarrierId { get; set; }
  public int? ActiveCarrierId { get; set; }
  public string? LocationName { get; set; }
  public string? Phone { get; set; }
}

public class CreateActiveCarrierPickupLocationCommandHandler : RequestHandlerBase<CreateActiveCarrierPickupLocationCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;
  private readonly ICountryRepository _countryRepository;

  public CreateActiveCarrierPickupLocationCommandHandler(ICarrierRepository carrierRepository,ICountryRepository countryRepository,IServiceProvider serviceProvider,ILogger<CreateActiveCarrierPickupLocationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateActiveCarrierPickupLocationCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new();
    try
    {
      string fullAddress = await _countryRepository.GetFullAddress(request.StreetAddress,request.CountryId,request.CityId,  request.AreaId,  request.ProvinceId, request.PinCodeId, request.StateId, request.EntityAddressDataJson );
      var code = ActiveCarrierPickupLocation.GetCode(request.LocationName!);

      if (request.ActiveCarrierPickupLocationId > 0) // update 
      {
        try
        {
          var pickupLocation = await _carrierRepository.GetActiveCarrierLocationbyId(request.ActiveCarrierPickupLocationId, _currentUser.ClientId!);
          if (pickupLocation == null)
          {
            throw new EntityNotFoundException("ActiveCarrierPickupLocation", request.ActiveCarrierPickupLocationId);
          }

          pickupLocation.UpdatepickLocation(
              request.LocationName,
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
              fullAddress,
              request.EntityAddressDataJson
          );

          var updated = await _carrierRepository.UpdatepickLocation(pickupLocation);
          if (updated != null)
          {
            var result = new BaseResponseDto
            {
              Data = updated.ActiveCarrierPickupLocationId,
              Message = NotificationConstants.Success
            };

            serviceResultDTO = new ServiceResultDTO(result);
            serviceResultDTO.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
            return serviceResultDTO;
          }
          else
          {
            serviceResultDTO.CreateErrorResponse(new Exception(NotificationConstants.Error));
            return serviceResultDTO;
          }
        }
        catch (Exception ex)
        {
          serviceResultDTO.CreateErrorResponse(ex);
          throw;
        }
      }
      else
      {
        ActiveCarrierPickupLocation acp = ActiveCarrierPickupLocation.CreatePickupLocation(fullAddress,request.CountryId, request.CityId, request.AreaId, request.StreetAddress, request.Latitude, request.Longitude, request.StreetAddress2, request.HouseNo,  request.BuildingName,request.Landmark, request.ProvinceId, request.PinCodeId, request.StateId, request.CarrierId.GetValueOrDefault(),  request.ActiveCarrierId.GetValueOrDefault(),  request.CustomerServiceNo,  request.EntityAddressDataJson, _currentUser.ClientId!, request.LocationName,request.Phone,
        code);

        var created = await _carrierRepository.CreateActiveCarrierPickupLocation(acp);
        serviceResultDTO = new ServiceResultDTO(new BaseResponseDto
        {
          Data = acp.ActiveCarrierPickupLocationId,
          Message = "Action performed successfully."
        });
        return serviceResultDTO;
      }
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}

public class CreateActiveCarrierPickupLocationCommandValidator : AbstractValidator<CreateActiveCarrierPickupLocationCommand>
{
  public CreateActiveCarrierPickupLocationCommandValidator()
  {
    RuleFor(x => x.CustomerServiceNo).NotEmpty().NotNull();
    RuleFor(x => x.CarrierId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.ActiveCarrierId).NotEmpty().NotNull().GreaterThan(0);
  }
}

