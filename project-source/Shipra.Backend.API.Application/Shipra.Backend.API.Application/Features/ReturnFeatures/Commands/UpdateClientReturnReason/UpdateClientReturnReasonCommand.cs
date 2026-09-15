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
using Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.CreateClientReturnReason;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.CreateReturn;
public class UpdateClientReturnReasonCommand : IRequest<ServiceResultDTO>
{
  public string? Reason { get; set; }
  public string? ReasonDetail { get; set; }
  public int ClientReturnReasonId { get; set; }
}
public class UpdateClientReturnReasonCommandHandler : RequestHandlerBase<UpdateClientReturnReasonCommand, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public UpdateClientReturnReasonCommandHandler(IReturnRepository returnRepository,IServiceProvider serviceProvider, ILogger<UpdateClientReturnReasonCommandHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientReturnReasonCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var order = await _returnRepository.GetClientReturnResonByClientReasonId(request.ClientReturnReasonId,_currentUser.ClientId!);
      if (order == null)
      {
        throw new EntityNotFoundException("ClientReturnReason ", request.ClientReturnReasonId!);
      }
      order.Update(request.Reason!, request.ReasonDetail!,_currentUser.ClientId!,_currentUser.EmployeeId!);
      bool res =  await _returnRepository.UpdateClientReturnReson(order); 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }  
  }
}
