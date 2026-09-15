using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllInvoiceStatus;
public class GetAllInvoiceStatusQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllInvoiceStatusQueryHandler : RequestHandlerBase<GetAllInvoiceStatusQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetAllInvoiceStatusQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<GetAllInvoiceStatusQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllInvoiceStatusQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _shipperInvoiceRepository.GetAllInvoiceStatus();
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
