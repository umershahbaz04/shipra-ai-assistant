using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperRateBySaleChannelConfig;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperRateBySaleChannelConfigGroupQuery;
public class GetShipperRateBySaleChannelConfigGroupQuery : IRequest<ServiceResultDTO>
{
}
public class GetShipperRateBySaleChannelConfigGroupHandler : RequestHandlerBase<GetShipperRateBySaleChannelConfigGroupQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetShipperRateBySaleChannelConfigGroupHandler(IShipperInvoiceRepository shipperInvoiceRepository, IServiceProvider serviceProvider, ILogger<GetShipperRateBySaleChannelConfigGroupHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetShipperRateBySaleChannelConfigGroupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mapData = await _shipperInvoiceRepository.GetShipperRateBySaleChannelConfig(0,_currentUser.ClientIdStr,0,100000,null,null);
      var dataList = mapData
          .GroupBy(x => x.SaleChannelConfigId)
          .Select(g => new
          {
            SaleChannelConfigId = g.Key,
            EmployeeName = g.First().EmployeeName 
          })
          .ToList();

      serviceResult = new ServiceResultDTO(dataList);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
