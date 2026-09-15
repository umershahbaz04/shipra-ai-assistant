using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetShipmentsByReturnReportId;
public class GetShipmentsByReturnReportIdQuery : IRequest<ServiceResultDTO>
{
  public string? CarrierRRId { get; set; }
}
public class GetShipmentsByReturnReportIdQueryHandler : RequestHandlerBase<GetShipmentsByReturnReportIdQuery, ServiceResultDTO>
{
  private readonly ICarrierReturnReport _carrierReturnReport;

  public GetShipmentsByReturnReportIdQueryHandler(ICarrierReturnReport carrierReturnReport, IServiceProvider serviceProvider, ILogger<GetShipmentsByReturnReportIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierReturnReport = carrierReturnReport;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetShipmentsByReturnReportIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var clientId = _currentUser.ClientId!.Value.ToString();

      dynamic data = await _carrierReturnReport.GetShipmentsByReturnReportId(request.CarrierRRId!, clientId);

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
public class GetShipmentsByReturnReportIdQueryValidator : AbstractValidator<GetShipmentsByReturnReportIdQuery>
{
  public GetShipmentsByReturnReportIdQueryValidator()
  {
    RuleFor(x => x.CarrierRRId).NotNull().NotEmpty();
  }
}
