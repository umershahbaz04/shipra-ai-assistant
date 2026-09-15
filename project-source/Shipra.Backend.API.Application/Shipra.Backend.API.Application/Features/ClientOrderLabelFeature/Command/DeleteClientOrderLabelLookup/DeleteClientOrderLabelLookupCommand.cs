using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.DeleteClientOrderLabelLookup;
public class DeleteClientOrderLabelLookupCommand : IRequest<ServiceResultDTO>
{
  public int ClientOrderLabelLookupId { get; set; }
}
public class DeleteClientOrderLabelLookupCommandHandler : RequestHandlerBase<DeleteClientOrderLabelLookupCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public DeleteClientOrderLabelLookupCommandHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<DeleteClientOrderLabelLookupCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteClientOrderLabelLookupCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var allClientOrderLabels = await _orderRepository.GetAllClientOrderLabelLookupForSelection(_currentUser.ClientId!);

      var oClientOrderLabelLookup = allClientOrderLabels.FirstOrDefault(x => x.ClientOrderLabelLookupId == request.ClientOrderLabelLookupId);
      if (oClientOrderLabelLookup is null)
      {
        throw new EntityNotFoundException("ClientOrderLabelLookup ", request.ClientOrderLabelLookupId!);
      }

      await _orderRepository.DeleteClientOrderLabelLookup(oClientOrderLabelLookup);

      serviceResult = new ServiceResultDTO(new BaseResponseDto { Message = "Labels added successfully" });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
