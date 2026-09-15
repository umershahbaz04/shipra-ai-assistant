using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetOrderInfoByOrderNoForPopUp;
public class GetOrderInfoByOrderNoForPopUpQueryHandler : RequestHandlerBase<GetOrderInfoByOrderNoForPopUpQuery, ServiceResultDTO>
{
  private readonly IKeyGeneratorService _keyGeneratorService;
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IShipmentRepository _shipmentRepository;

  public GetOrderInfoByOrderNoForPopUpQueryHandler(IKeyGeneratorService keyGeneratorService,IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IOrderRepository orderRepository, IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<GetOrderInfoByOrderNoForPopUpQueryHandler> logger) : base(serviceProvider, logger)
  {
    _keyGeneratorService = keyGeneratorService;
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _orderRepository = orderRepository;
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderInfoByOrderNoForPopUpQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {

      var orderInfoForPopUp = await _shipmentRepository.GetOrderInfoForPopupByOrderNo(request?.OrderNo!, _currentUser!.ClientIdStr!);
      if (orderInfoForPopUp is null)
      {
        throw new EntityNotFoundException("Order", request!.OrderNo!);
      }
      var orderTrackingHistory = await _orderTrackingHistoryRepository.GetOrderTrackingHistoryByOrderNo(request?.OrderNo!, _currentUser!.ClientIdStr!);
      var orderNote = await _orderTrackingHistoryRepository.GetOrderNoteByOrderNo(request?.OrderNo!, _currentUser!.ClientIdStr!);
      var orderItemsInfo = await _orderRepository.GetOrderItemsInfoByOrderId(orderInfoForPopUp!.OrderId!.ToString(), _currentUser.ClientIdStr!);
      var mapLocation = await _shipmentRepository.GetOrderAddressInforForMapByOrderId(new OrderId(orderInfoForPopUp!.OrderId!));
      var podFiles = await _shipmentRepository.GetOrderPodFilesByOrderId(new OrderId(orderInfoForPopUp!.OrderId!));
      var ActiveCarrierPickupLocationinfo = await _shipmentRepository.GetActiveCarrierPickupLocationById(orderInfoForPopUp!.ActiveCarrierPickupLocationId!);

      var trackingPageUrl = _configuration.GetValue<string>("TrackingPageUrl");

      var trackingModel = new TrackingModel
      {
        ClientId = _currentUser.ClientIdStr,
        OrderNo = request?.OrderNo
      };
      var param = $"{_currentUser.ClientIdStr}_{request?.OrderNo}";
      var url = $"{trackingPageUrl}/{param}";
      serviceResult = new ServiceResultDTO(new
      {
        order = orderInfoForPopUp,
        orderNote,
        orderItemsInfo,
        ActiveCarrierPickupLocationinfo,
        orderTrackingHistory,
        mapLocation,
        trackUrl = url,
        podFiles = podFiles.Select(x => new
        {
          orderPodfileId = x.OrderPodfileId?.Value!.ToString(),
          x.FilePath,
          x.Comment,
          orderId = x.OrderId!.Value!.ToString(),
        }).ToList(),
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
