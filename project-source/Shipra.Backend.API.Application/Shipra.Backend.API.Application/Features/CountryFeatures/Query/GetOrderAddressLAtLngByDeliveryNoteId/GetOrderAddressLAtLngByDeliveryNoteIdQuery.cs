using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DeliveryNoteUseCase.Response;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetOrderAddressLAtLngByDeliveryNoteId;
public class GetOrderAddressLAtLngByDeliveryNoteIdQuery : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteId { get; set; }
}
public class GetOrderAddressLAtLngByDeliveryNoteIdQueryHandler : RequestHandlerBase<GetOrderAddressLAtLngByDeliveryNoteIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public GetOrderAddressLAtLngByDeliveryNoteIdQueryHandler(ICountryRepository countryRepository, IOrderRepository orderRepository, IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<GetOrderAddressLAtLngByDeliveryNoteIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
    _orderRepository = orderRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderAddressLAtLngByDeliveryNoteIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var deliveryNoteId = new DeliveryNoteId(new Guid(request?.DeliveryNoteId!));
      var deliveryNote = await _deliveryNoteRepository.GetDeliveryNoteById(deliveryNoteId);
      if (deliveryNote is not null)
      {
        var deliveryNoteDelivery = await _deliveryNoteRepository.GetAllDeliveryNoteDetailByNoteId(deliveryNoteId)!;

        var orderIDs = string.Join(",", deliveryNoteDelivery.Select(x => x.OrderId!.Value!.ToString()).ToList());
        var orders = await _orderRepository.GetOrdersByOrderIds(orderIDs, _currentUser.ClientId!);

        var orderResults = new List<object>();

        foreach (var order in orders)
        {
          var orderAddress = await _orderRepository.GetOrderAddressById(order.OrderAddressId.GetValueOrDefault());

          if (orderAddress is not null)
          {
            double? latitude = orderAddress.Latitude.HasValue ? (double?)orderAddress.Latitude.Value : null;
            double? longitude = orderAddress.Longitude.HasValue ? (double?)orderAddress.Longitude.Value : null;

            if (latitude == null || longitude == null)
            {
              // Try to fetch coordinates if missing
              (latitude, longitude) = await _countryRepository.GetCoordinatesByAddressAsync(orderAddress.CustomerFullAddress!);
            }

            var data = new
            {
              OrderNo = order.OrderNo,
              isValid = latitude != null && longitude != null,
              Lat = latitude,
              Lng = longitude
            };

            orderResults.Add(data);
          }
        }

        serviceResult = new ServiceResultDTO(orderResults);
      }
      else
      {
        throw new EntityNotFoundException("DeliveryNote ", deliveryNoteId!.Value);
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
public class GetOrderAddressLAtLngByDeliveryNoteIdQueryValidator : AbstractValidator<GetOrderAddressLAtLngByDeliveryNoteIdQuery>
{
  public GetOrderAddressLAtLngByDeliveryNoteIdQueryValidator()
  {
    RuleFor(x => x.DeliveryNoteId).NotEmpty().NotNull();
  }
}
