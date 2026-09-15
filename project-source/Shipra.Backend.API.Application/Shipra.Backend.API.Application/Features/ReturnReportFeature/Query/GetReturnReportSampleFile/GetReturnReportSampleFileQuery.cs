using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetReturnReportSampleFile;
public class GetReturnReportSampleFileQuery : IRequest<ServiceResultDTO>
{
}
public class GetReturnReportSampleFileQueryHandler : RequestHandlerBase<GetReturnReportSampleFileQuery, ServiceResultDTO>
{
  public GetReturnReportSampleFileQueryHandler(IServiceProvider serviceProvider, ILogger<GetReturnReportSampleFileQueryHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetReturnReportSampleFileQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      await Task.Delay(1);
      string fileUrl = ApplicationConstants.ReturnReportFileUrl; 
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
