using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetAllMyCarrierReturnReports;
public class GetAllMyCarrierReturnReportsQueryHandler : RequestHandlerBase<GetAllMyCarrierReturnReportsQuery, ServiceResultDTO>
{
  private readonly ICarrierReturnReport _carrierReturnReport;
  private readonly IClientRepository _clientRepository;

  public GetAllMyCarrierReturnReportsQueryHandler(ICarrierReturnReport carrierReturnReport, IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<GetAllMyCarrierReturnReportsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierReturnReport = carrierReturnReport;
    _clientRepository = clientRepository;

  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllMyCarrierReturnReportsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      var oClient = await _clientRepository.GetClientById(_currentUser.ClientId!);
      var oReturnReportList = await _carrierReturnReport.GetAllMyCarrierReturnReport(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, oClient!.DefaultCarrierId!, _currentUser.ClientIdStr!);
      if (oReturnReportList is not null)
      {
        serviceResult = new ServiceResultDTO(oReturnReportList);
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
