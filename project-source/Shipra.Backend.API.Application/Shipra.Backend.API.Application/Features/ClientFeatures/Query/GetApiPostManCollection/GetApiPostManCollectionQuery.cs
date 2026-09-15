using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetApiPostManCollection;
public class GetApiPostManCollectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetApiPostManCollectionQueryHandler : RequestHandlerBase<GetApiPostManCollectionQuery, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;

  public GetApiPostManCollectionQueryHandler(IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<GetApiPostManCollectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetApiPostManCollectionQuery request, CancellationToken cancellationToken)
  {
    await Task.Delay(500);
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"ReSource");
    string templatePath = Path.Combine(accessUploadsFolder, $"apiCollection.json");

    byte[] fileBytes = File.ReadAllBytes(templatePath);
    serviceResult = new ServiceResultDTO(fileBytes);
    return serviceResult!;
  }
}
