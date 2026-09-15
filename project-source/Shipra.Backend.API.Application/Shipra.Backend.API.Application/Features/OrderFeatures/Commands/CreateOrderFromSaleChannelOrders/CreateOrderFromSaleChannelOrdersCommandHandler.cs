using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderFromSaleChannelOrders;
public class CreateOrderFromSaleChannelOrdersCommandHandler : RequestHandlerBase<CreateOrderFromSaleChannelOrdersCommand, ServiceResultDTO>
{
  private readonly ISaleChannelOrderRepository _channelOrderRepository;
  public CreateOrderFromSaleChannelOrdersCommandHandler(ISaleChannelOrderRepository channelOrderRepository, IServiceProvider serviceProvider, ILogger<CreateOrderFromSaleChannelOrdersCommandHandler> logger) : base(serviceProvider, logger)
  {
    _channelOrderRepository = channelOrderRepository;
  }
  protected override Task<ServiceResultDTO> HandleRequest(CreateOrderFromSaleChannelOrdersCommand request, CancellationToken cancellationToken)
  {
    throw new Exception();
  }
}
