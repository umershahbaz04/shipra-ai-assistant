using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UploadProductMedia;
public class UploadProductMediaCommand : IRequest<ServiceResultDTO>
{
}
public class UploadProductMediaCommandHandler : RequestHandlerBase<UploadProductMediaCommand, ServiceResultDTO>
{
  public UploadProductMediaCommandHandler(IServiceProvider serviceProvider, ILogger<UploadProductMediaCommandHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override Task<ServiceResultDTO> HandleRequest(UploadProductMediaCommand request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
} 
