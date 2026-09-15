using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.CustomBinder;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateValidatedOrderForCarrier;
public class UpdateValidatedOrderAddressForCarrierQuery : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
  public string? StreetAddress { get; set; }
  public int? CarrierId { get; set; }
  public Dictionary<string,string>? Address { get; set; }
}
public class UpdateValidatedOrderAddressForCarrierQueryHandler : RequestHandlerBase<UpdateValidatedOrderAddressForCarrierQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository; 
  private readonly ICountryRepository _countryRepository;
  private readonly IOrderRepository _orderRepository;

  public UpdateValidatedOrderAddressForCarrierQueryHandler(IEmployeeRepository employeeRepository, ICountryRepository countryRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<UpdateValidatedOrderAddressForCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository; 
    _countryRepository = countryRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateValidatedOrderAddressForCarrierQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    {
      var orderId = new OrderId(new Guid(request!.OrderId!));
      var order = await _orderRepository.GetOrderById(orderId, _currentUser.ClientId!);
      if (order == null)
      {
        throw new EntityNotFoundException("Order ", orderId.Value);
      }
      if (!order!.TrackingLock.GetValueOrDefault(false))
      {
        var orderAddress = await _orderRepository.GetOrderAddressById(order!.OrderAddressId.GetValueOrDefault());
        if (orderAddress == null)
        {
          throw new EntityNotFoundException("OrderAddress ", order!.OrderAddressId.GetValueOrDefault());
        } 
         
        OrderAddressClientSideModel addressModel = MapToOrderAddressClientSideModel(request.Address!);
        
        var objOrderAddress = AddressConversionHelper.ConvertToOrderAddressModel(addressModel)!;
        objOrderAddress.CountryId = orderAddress.CountryId;
        objOrderAddress.SelectedCarrierId = request.CarrierId;
        #region order address  
        #region update street address 2 
        string? modifyStreet = !string.IsNullOrEmpty(request.StreetAddress) ? request.StreetAddress : orderAddress.StreetAddress ;
        #endregion
        string fullAddress = await _countryRepository.GetFullAddress(modifyStreet, orderAddress.CountryId, objOrderAddress.CityId, objOrderAddress.AreaId, objOrderAddress.ProvinceId, objOrderAddress.PinCodeId, objOrderAddress.StateId,objOrderAddress.EntityAddressDataJson);

        orderAddress.UpdateValidatedAddressForCarrier(fullAddress, objOrderAddress.CityId, objOrderAddress!.AreaId, objOrderAddress!.ProvinceId, objOrderAddress!.PinCodeId, objOrderAddress.StateId, objOrderAddress.SelectedCarrierId, objOrderAddress.EntityAddressDataJson, modifyStreet);
        orderAddress = await _orderRepository.UpdateOrderAddress(orderAddress!);
        #endregion

        #region order history
        if (!order!.TrackingLock.GetValueOrDefault(false))
        {
          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

          OrderNote orderHistory = OrderNote.CreateOrderNote(order!.OrderId!, "", _currentUser.EmployeeId!);

          OrderNote createdOrderTrackingHistory = await _orderRepository.CreateOrderNote(orderHistory);
        }
        #endregion 
        response = new ServiceResultDTO(new BaseResponseDto { Data = "", Message = NotificationConstants.Success });
      }
      else
      {
        response.CreateError("OrderLocked", new string[] { "Order already locked" });
      }

      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
  public static OrderAddressClientSideModel MapToOrderAddressClientSideModel(Dictionary<string, string> entityValues)
  {
    var model = new OrderAddressClientSideModel();
    Type modelType = typeof(OrderAddressClientSideModel);

    foreach (var entry in entityValues)
    {
      string propertyName = CivilEntityHelper.GetTableEntityPropertyFromKey(entry.Key, false);

      // Fetch only properties declared in OrderAddressClientSideModel
      PropertyInfo? property = modelType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

      if (property != null && property.CanWrite)
      {
        property.SetValue(model, entry.Value);
      }
    }

    return model;
  }

}
public class UpdateValidatedOrderAddressForCarrierQueryValidator : AbstractValidator<UpdateValidatedOrderAddressForCarrierQuery>
{
  public UpdateValidatedOrderAddressForCarrierQueryValidator()
  {
    RuleFor(x => x.OrderId).NotEmpty().NotNull();
    RuleFor(x => x.StreetAddress).NotEmpty().NotNull();
    RuleFor(x => x.Address).NotEmpty().NotNull();
  } 
}
