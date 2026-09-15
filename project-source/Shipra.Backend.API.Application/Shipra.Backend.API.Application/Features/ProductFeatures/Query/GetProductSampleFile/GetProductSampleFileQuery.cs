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
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductSampleFile;
public class GetProductSampleFileQuery : IRequest<ServiceResultDTO>
{
}
public class GetProductSampleFileQueryHandler : RequestHandlerBase<GetProductSampleFileQuery, ServiceResultDTO>
{
  public GetProductSampleFileQueryHandler(IServiceProvider serviceProvider, ILogger<GetProductSampleFileQueryHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetProductSampleFileQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      await Task.Delay(1);
      string? fileUrl = "https://shipprastatic.s3.ap-south-1.amazonaws.com/Shipra/Others/other/product/13-02-2025/f1d96724-5c99-4f1b-bbad-bd55a0c5cb68_sample-product.xlsx"; 
      serviceResult = new ServiceResultDTO(new { url = fileUrl });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
