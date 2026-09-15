using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetReturnRerports;
public class GetReturnRerportsQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetReturnRerportsQueryHandler : RequestHandlerBase<GetReturnRerportsQuery, ServiceResultDTO>
{
  private readonly ICarrierReturnReport _carrierReturnReport;

  public GetReturnRerportsQueryHandler(ICarrierReturnReport carrierReturnReport, IServiceProvider serviceProvider, ILogger<GetReturnRerportsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierReturnReport = carrierReturnReport;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetReturnRerportsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!; 
      dynamic data = await _carrierReturnReport.GetReturnRerports(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,_currentUser.ClientIdStr!);

      serviceResult = new ServiceResultDTO(data);
      serviceResult.CreateSuccessResponse();
      return serviceResult;

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
