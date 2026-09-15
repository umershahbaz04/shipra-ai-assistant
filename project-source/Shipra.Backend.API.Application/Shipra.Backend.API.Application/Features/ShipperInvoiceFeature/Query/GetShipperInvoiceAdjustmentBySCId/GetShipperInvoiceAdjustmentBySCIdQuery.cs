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
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperInvoiceAdjustmentBySCId;
public class GetShipperInvoiceAdjustmentBySCIdQuery : IRequest<ServiceResultDTO>
{
  public int? SaleChannelConfigId { get; set; }
}
public class GetShipperInvoiceAdjustmentBySCIdQueryHandler : RequestHandlerBase<GetShipperInvoiceAdjustmentBySCIdQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetShipperInvoiceAdjustmentBySCIdQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<GetShipperInvoiceAdjustmentBySCIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetShipperInvoiceAdjustmentBySCIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var mapData = await _shipperInvoiceRepository.GetShipperInvoiceAdjustmentBySCId(request.SaleChannelConfigId.GetValueOrDefault(), _currentUser.ClientId!.Value); 
      serviceResult = new ServiceResultDTO(mapData!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
