using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderDraft;
public class CreateOrderDraftCommand : IRequest<ServiceResultDTO>
{
  public long? OrderDraftId { get; set; }  
  public string? OrderInfo { get;  set; }
  public int? OrderTypeId { get; set; }
}
public class CreateOrderDraftCommandHandler : RequestHandlerBase<CreateOrderDraftCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public CreateOrderDraftCommandHandler(IOrderRepository orderRepository,IServiceProvider serviceProvider, ILogger<CreateOrderDraftCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateOrderDraftCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var existDraftOrder = await _orderRepository.GetDraftOrderById(request.OrderDraftId.GetValueOrDefault(), _currentUser.ClientId!);

      if (existDraftOrder is null)
      {
        var orderDrafts = await _orderRepository.GetAllDraftOrders(_currentUser.ClientId!);
        orderDrafts = orderDrafts.OrderByDescending(od => od.OrderDraftId).ToList();
        var firstODraft = orderDrafts.FirstOrDefault();  
        var oOrderDraft = OrderDraft.Create(OrderDraft.GetOrderNo(firstODraft!), request.OrderInfo!,request.OrderTypeId.GetValueOrDefault(), _currentUser.ClientId!, _currentUser.EmployeeId);

        var isCreated = await _orderRepository.CreateOrderDraft(oOrderDraft);
        if (isCreated)
        {
          serviceResult = new ServiceResultDTO(new BaseResponseDto
          {
            Data = new { oOrderDraft.OrderDraftId},
            Message = "Created Successfully"
          });
        }
      }
      else
      {
        existDraftOrder.Update(request.OrderInfo!,_currentUser.EmployeeId);
        var isUpdate = await _orderRepository.UpdateOrderDraft(existDraftOrder);
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = new { existDraftOrder.OrderDraftId },
          Message = "Updated Successfully"
        });
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
