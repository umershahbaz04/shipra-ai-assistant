using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ExampleFeatures.Commands;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllCivilEntityExtendedByCarrier;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.ClientAddressFromAdmin;
public class GetClientAddressFromAdminQuery : IRequest<ServiceResultDTO>
{
  public string? AddressFrom { get; set; }
  public string? AddressTo { get; set; }
}
    public class GetClientAddressFromAdminQueryQueryHandler : Common.RequestHandlerBase<GetClientAddressFromAdminQuery, ServiceResultDTO>
  {
  private readonly ICountryRepository _countryRepository;
  private readonly ICarrierRepository _carrierRepository;

    public GetClientAddressFromAdminQueryQueryHandler(ICountryRepository countryRepository, ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetClientAddressFromAdminQueryQueryHandler> logger) : base(serviceProvider, logger)
    {
    _countryRepository = countryRepository;
    _carrierRepository = carrierRepository;
    }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientAddressFromAdminQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      await Task.Delay(1);

      // Parse AddressFrom
      decimal fromLatitude = 0, fromLongitude = 0;
      if (!string.IsNullOrWhiteSpace(request.AddressFrom))
      {
        var fromParts = request.AddressFrom.Split(',');
        if (fromParts.Length == 2 &&
            decimal.TryParse(fromParts[0].Trim(), out var lat) &&
            decimal.TryParse(fromParts[1].Trim(), out var lng))
        {
          fromLatitude = lat;
          fromLongitude = lng;
        }
        else
        {
          throw new ArgumentException("Invalid AddressFrom format. Expected 'lat,long'.");
        }
      }

      // Parse AddressTo
      decimal toLatitude = 0, toLongitude = 0;
      if (!string.IsNullOrWhiteSpace(request.AddressTo))
      {
        var toParts = request.AddressTo.Split(',');
        if (toParts.Length == 2 &&
            decimal.TryParse(toParts[0].Trim(), out var lat) &&
            decimal.TryParse(toParts[1].Trim(), out var lng))
        {
          toLatitude = lat;
          toLongitude = lng;
        }
        else
        {
          throw new ArgumentException("Invalid AddressTo format. Expected 'lat,long'.");
        }
      }

      dynamic res1 = await _countryRepository.GetCivilEntityIdByLatitudeAndLongitude(fromLatitude, fromLongitude, null);
      dynamic res2 = await _countryRepository.GetCivilEntityIdByLatitudeAndLongitude(toLatitude, toLongitude, null);

      var filter = new PriceCalculatorFilter
      {
        From = res1?.Country,
        To = res2?.Country,    
        Weight = 1,
        ClientId = _currentUser.ClientIdStr,
      };


      var data = await _carrierRepository.GetAllClientRateAsync(filter);
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

}
public class ExampleCommandValidator : AbstractValidator<ExampleCommand>
  {
    public ExampleCommandValidator()
    {
      RuleFor(e => e.Name).NotEmpty();
    }
  }

public class AdressCommandValidator : AbstractValidator<GetClientAddressFromAdminQuery>
{
  public AdressCommandValidator()
  {
    RuleFor(e => e.AddressFrom).NotNull().NotEmpty();
    RuleFor(e => e.AddressTo).NotNull().NotEmpty();
  }
}
