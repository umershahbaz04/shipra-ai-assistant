using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.CreateClientReturnReason;
public class CreateClientReturnReasonCommand : IRequest<ServiceResultDTO>
{
  public string? Reason { get; set; }
  public string? ReasonDetail { get; set; }
}
public class CreateClientReturnReasonCommandHandler : RequestHandlerBase<CreateClientReturnReasonCommand, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public CreateClientReturnReasonCommandHandler(IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<CreateClientReturnReasonCommandHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientReturnReasonCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var clientReturnReason = ClientReturnReason.Create(request.Reason!, request.ReasonDetail!, _currentUser.ClientId!, _currentUser.EmployeeId!);

      await _returnRepository.CreateClientReturnReson(clientReturnReason);
      await Task.Delay(1);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
