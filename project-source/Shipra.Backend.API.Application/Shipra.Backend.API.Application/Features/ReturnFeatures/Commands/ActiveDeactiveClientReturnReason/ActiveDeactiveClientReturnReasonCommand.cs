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
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.ActiveDeactiveClientReturnReason;
public class ActiveDeactiveClientReturnReasonCommand : IRequest<ServiceResultDTO>
{
  public bool? IsActive { get; set; }
  public int ClientReturnReasonId { get; set; }
}
public class ActiveDeactiveClientReturnReasonCommandHandler : RequestHandlerBase<ActiveDeactiveClientReturnReasonCommand, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public ActiveDeactiveClientReturnReasonCommandHandler(IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<ActiveDeactiveClientReturnReasonCommandHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ActiveDeactiveClientReturnReasonCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var order = await _returnRepository.GetClientReturnResonByClientReasonId(request.ClientReturnReasonId, _currentUser.ClientId!);
      if (order == null)
      {
        throw new EntityNotFoundException("ClientReturnReason ", request.ClientReturnReasonId!);
      }
      order.ActivateDeactive(_currentUser.EmployeeId!, request.IsActive ?? false);
      var res = await _returnRepository.UpdateClientReturnReson(order);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

