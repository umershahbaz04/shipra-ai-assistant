using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using DocumentFormat.OpenXml.EMMA;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAwbForCarrierByOrderNos;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAwbForCarrierByRefNos;
public class GetAwbForCarrierByRefNosQuery : IRequest<ServiceResultDTO>
{
  public string? RefNos { get; set; }
}
public class GetAwbForCarrierByRefNosQueryHandler : RequestHandlerBase<GetAwbForCarrierByRefNosQuery, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IOrderRepository _orderRepository;

  public GetAwbForCarrierByRefNosQueryHandler(IMediator mediator, IOrderRepository orderRepository,IServiceProvider serviceProvider, ILogger<GetAwbForCarrierByRefNosQueryHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAwbForCarrierByRefNosQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new();
    try
    {
      List<Order> orders = await _orderRepository.GetAllOrdersByRefNos(request.RefNos, _currentUser.ClientId);
      if (orders.Count > 0)
      {
        GetAwbForCarrierByOrderNosQuery requestModel = new GetAwbForCarrierByOrderNosQuery();
        requestModel.orderNos = string.Join(",", orders.Select(x => x.OrderNo).ToList());
        serviceResultDTO = await _mediator.Send(requestModel);
      
      }
      return serviceResultDTO;
    }
    catch (Exception)
    {

      throw;
    }
  }
}

public class GetAwbForCarrierByRefNosQueryValidator : AbstractValidator<GetAwbForCarrierByRefNosQuery>
{
  public GetAwbForCarrierByRefNosQueryValidator()
  {
    RuleFor(x => x.RefNos).NotNull().NotEmpty();
  }
}
