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

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllCarrierRateWithContractData;
public class GetAllCarrierRateWithContractDataQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int? DeliveryServiceId { get; set; }
  public int? SaleChannelConfigId { get; set; }
}
public class GetAllCarrierRateWithContractDataQueryHandler : RequestHandlerBase<GetAllCarrierRateWithContractDataQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetAllCarrierRateWithContractDataQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<GetAllCarrierRateWithContractDataQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCarrierRateWithContractDataQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var data = await _shipperInvoiceRepository.GetAllCarrierRateWithContractDataAsync(filter.Start, filter.Length, filter.Search!,request.DeliveryServiceId,_currentUser.ClientIdStr,request.SaleChannelConfigId);
      serviceResult = new ServiceResultDTO(data!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
