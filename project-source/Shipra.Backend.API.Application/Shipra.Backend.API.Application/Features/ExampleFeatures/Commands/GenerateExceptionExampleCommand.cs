using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ExampleFeatures.Commands;
public class GenerateExceptionExampleCommand : IRequest<ServiceResultDTO>
{ 
}
public class GenerateExceptionExampleCommandHandler : RequestHandlerBase<GenerateExceptionExampleCommand, ServiceResultDTO>
{
  public GenerateExceptionExampleCommandHandler(IServiceProvider serviceProvider, ILogger<GenerateExceptionExampleCommandHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GenerateExceptionExampleCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      await Task.Delay(1); 
      var data = 0 / 1;
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
