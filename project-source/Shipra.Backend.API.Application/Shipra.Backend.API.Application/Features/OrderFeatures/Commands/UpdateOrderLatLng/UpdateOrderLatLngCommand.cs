using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Application.Common.CustomBinder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateValidatedOrderForCarrier;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderLatLng;
public class UpdateOrderLatLngCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
  public decimal? Longitude { get; set; }
  public decimal? Latitude { get; set; }
  public string? StreetAddress { get; set; } 
  public Dictionary<string, string>? Address { get; set; }
}
public class UpdateOrderLatLngCommandHandler : RequestHandlerBase<UpdateOrderLatLngCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;
  private readonly ICountryRepository _countryRepository;

  public UpdateOrderLatLngCommandHandler(IOrderRepository orderRepository, ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<UpdateOrderLatLngCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateOrderLatLngCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (order is null)
      {
        throw new EntityNotFoundException("Order ", request.OrderNo!);
      }

      var oOrderAddress = await _orderRepository.GetOrderAddressById(order.OrderAddressId.GetValueOrDefault());
      if (oOrderAddress is null)
      {
        serviceResult.CreateError("OrderAddress", new string[] { "Order address not found against order no " + request.OrderNo! });
      }

      decimal? safeLat = request.Latitude.HasValue ? Math.Round(request.Latitude.Value, 6) : null;
      decimal? safeLng = request.Longitude.HasValue ? Math.Round(request.Longitude.Value, 6) : null;

      oOrderAddress!.UpdateLatLng(safeLat, safeLng, _currentUser.EmployeeId);

      if (request.Address != null && request.Address.Count > 0)
      {
        var addressModel = UpdateValidatedOrderAddressForCarrierQueryHandler.MapToOrderAddressClientSideModel(request.Address);
        var objOrderAddress = AddressConversionHelper.ConvertToOrderAddressModel(addressModel)!;

        var countryIdToUse = objOrderAddress.CountryId ?? oOrderAddress.CountryId;
        var country = await _countryRepository.GetCountryById(countryIdToUse);
        if (country != null && !string.IsNullOrEmpty(country.AddressingScheme))
        {
          var counrtyKeyList = Newtonsoft.Json.JsonConvert.DeserializeObject<Shipra.Backend.API.Core.Models.CountryAddressSchema>(country.AddressingScheme);
          if (counrtyKeyList != null && counrtyKeyList.keys != null)
          {
            foreach (var key in counrtyKeyList.keys)
            {
              bool isRequired = Shipra.Backend.API.Core.Helper.CivilEntityHelper.CheckRequiredKeyInCountryAddressingShceck(country.AddressingScheme, key);
              if (isRequired)
              {
                if (!request.Address.ContainsKey(key) || string.IsNullOrEmpty(request.Address[key]))
                {
                  serviceResult.CreateError(key, new string[] { $"{char.ToUpper(key[0])}{key.Substring(1)} is required" });
                }
              }
            }
          }
        }

        if (serviceResult.Errors != null && serviceResult.Errors.Count > 0)
        {
          return serviceResult;
        }

        string? modifyStreet = !string.IsNullOrEmpty(request.StreetAddress) ? request.StreetAddress : oOrderAddress.StreetAddress;

        string fullAddress = await _countryRepository.GetFullAddress(modifyStreet, oOrderAddress.CountryId, objOrderAddress.CityId, objOrderAddress.AreaId, objOrderAddress.ProvinceId, objOrderAddress.PinCodeId, objOrderAddress.StateId, objOrderAddress.EntityAddressDataJson);

        oOrderAddress.UpdateAddressFromDeliveryTask(fullAddress, objOrderAddress.CityId, objOrderAddress.AreaId, objOrderAddress.ProvinceId, objOrderAddress.PinCodeId, objOrderAddress.StateId, objOrderAddress.EntityAddressDataJson, modifyStreet);
      }

      await _orderRepository.UpdateOrderAddress(oOrderAddress);
      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Data =
        new
        {
          lat = safeLat,
          lng = safeLng,
          request.OrderNo
        },
        Message = "Lat Lng update successfully"
      });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class UpdateOrderLatLngCommandHandlerValidator : AbstractValidator<UpdateOrderLatLngCommand>
{
  public UpdateOrderLatLngCommandHandlerValidator()
  {
    When(v => v.Latitude != null && v.Longitude != null, () =>
    {
      RuleFor(v => v.Longitude).InclusiveBetween(-180m, 180m).WithMessage("Invalid Longitude");
      RuleFor(v => v.Latitude).InclusiveBetween(-90m, 90m).WithMessage("Invalid Latitude");
    });
  }
}
