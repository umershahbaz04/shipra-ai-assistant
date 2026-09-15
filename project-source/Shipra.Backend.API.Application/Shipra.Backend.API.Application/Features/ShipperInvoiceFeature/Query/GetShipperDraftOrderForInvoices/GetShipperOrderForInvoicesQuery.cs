using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperDraftOrderForInvoices;
public class GetShipperOrderForInvoicesQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int? SaleChannelConfigId { get; set; }
}
public class GetShipperOrderForInvoicesQueryHandler : RequestHandlerBase<GetShipperOrderForInvoicesQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetShipperOrderForInvoicesQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<GetShipperOrderForInvoicesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetShipperOrderForInvoicesQuery request, CancellationToken cancellationToken)
  {
    var result = new ServiceResultDTO();

    try
    {
      var status = (int)EnumInvoiceStatus.Draft;
      var filter = request.FilterModel!;
      var shipperOrderForInvoices = await _shipperInvoiceRepository.GetShipperOrderForInvoices(filter.Start,filter.Length,filter.Search, status.ToString(),request.SaleChannelConfigId, _currentUser.ClientIdStr,filter.CreatedFrom,filter.CreatedTo);
       
      result = new ServiceResultDTO(shipperOrderForInvoices);
      return result;
    }
    catch (Exception ex)
    {
      result.CreateErrorResponse(ex);
      return result;
    }
  }
}
