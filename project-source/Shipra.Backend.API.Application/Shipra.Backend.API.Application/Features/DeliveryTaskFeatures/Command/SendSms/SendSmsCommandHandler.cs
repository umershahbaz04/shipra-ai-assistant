using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.RevertDeliveryTask;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.SendSms;
public class SendSmsCommandHandler : RequestHandlerBase<SendSmsCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IOrderRepository _orderRepository;
  private readonly IZokoService _zokoWhatsAppService;

  public SendSmsCommandHandler(IMediator mediator, IOrderRepository orderRepository, IZokoService zokoService, IServiceProvider serviceProvider, ILogger<SendSmsCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _orderRepository = orderRepository;
    _zokoWhatsAppService = zokoService;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SendSmsCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);

      if (order == null)
      {
        throw new EntityNotFoundException("Order not found", request.OrderNo!);
      }

      var orderAddress = await _orderRepository.GetOrderAddressById(order!.OrderAddressId ?? 0);

      var result = await _zokoWhatsAppService.SendLocationNotificationAsync(order, orderAddress, _currentUser.ClientId!);

      if (!result.result)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = order.OrderNo,
          Message = result.message
        });

        return serviceResult;
      }

      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Data = order.OrderNo,
        Message = "WhatsApp notification sent successfully."
      });

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
