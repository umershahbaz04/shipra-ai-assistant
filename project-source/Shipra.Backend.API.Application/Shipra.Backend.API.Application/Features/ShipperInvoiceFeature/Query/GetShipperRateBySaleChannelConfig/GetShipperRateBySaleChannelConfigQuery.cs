using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperInvoiceAdjustmentById;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperRateBySaleChannelConfig;
public class GetShipperRateBySaleChannelConfigQuery : IRequest<ServiceResultDTO>
{
  public int? SaleChannelConfigId { get; set; }
}
public class GetShipperRateBySaleChannelConfigQueryHandler : RequestHandlerBase<GetShipperRateBySaleChannelConfigQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetShipperRateBySaleChannelConfigQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository, IServiceProvider serviceProvider, ILogger<GetShipperRateBySaleChannelConfigQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetShipperRateBySaleChannelConfigQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mapData = await _shipperInvoiceRepository.GetShipperRateBySaleChannelConfig(request.SaleChannelConfigId.GetValueOrDefault(), _currentUser.ClientIdStr,0,100,null,null);
      if (mapData is null)
      {
        throw new EntityNotFoundException("Error ", request.SaleChannelConfigId!);
      }
      serviceResult = new ServiceResultDTO(mapData);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
