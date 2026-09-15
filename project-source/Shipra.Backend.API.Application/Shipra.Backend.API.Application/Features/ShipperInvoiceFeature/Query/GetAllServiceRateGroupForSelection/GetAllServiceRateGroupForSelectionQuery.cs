using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllServiceRateGroupForSelection;
public class GetAllServiceRateGroupForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllServiceRateGroupForSelectionQueryHandler : RequestHandlerBase<GetAllServiceRateGroupForSelectionQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetAllServiceRateGroupForSelectionQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository, IServiceProvider serviceProvider, ILogger<GetAllServiceRateGroupForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllServiceRateGroupForSelectionQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _shipperInvoiceRepository.GetAllServiceRateGroupForSelection(_currentUser.ClientId!.Value);

      var groupedData = data.GroupBy(x => x.Code).ToList();
      var list = groupedData.Select(g => new
      {
        id = g.Key,
        Code = g.Key,
      }).ToList();
      serviceResult = new ServiceResultDTO(list!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
