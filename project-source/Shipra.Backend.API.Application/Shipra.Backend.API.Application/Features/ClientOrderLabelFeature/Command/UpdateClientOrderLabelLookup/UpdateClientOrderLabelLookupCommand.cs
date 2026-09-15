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
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.UpdateClientOrderLabelLookup;
public class UpdateClientOrderLabelLookupCommand : IRequest<ServiceResultDTO>
{
  public int ClientOrderLabelLookupId { get; set; }
  public string? LabelName { get; set; } 
  public string? ColorCode { get; set; } 
}
public class UpdateClientOrderLabelLookupCommandHandler : RequestHandlerBase<UpdateClientOrderLabelLookupCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public UpdateClientOrderLabelLookupCommandHandler(IOrderRepository orderRepository,IServiceProvider serviceProvider, ILogger<UpdateClientOrderLabelLookupCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientOrderLabelLookupCommand request, CancellationToken cancellationToken)
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
      oClientOrderLabelLookup.Update(request.LabelName, request.ColorCode, _currentUser.EmployeeId!);
      await _orderRepository.UpdateClientOrderLabelLookup(oClientOrderLabelLookup);

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
