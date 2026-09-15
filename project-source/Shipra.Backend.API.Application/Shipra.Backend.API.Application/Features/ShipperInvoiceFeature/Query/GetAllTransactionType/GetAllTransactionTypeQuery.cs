using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllInvoiceStatus;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllTransactionType;
public class GetAllTransactionTypeQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllTransactionTypeQueryHandler : RequestHandlerBase<GetAllTransactionTypeQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetAllTransactionTypeQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository, IServiceProvider serviceProvider, ILogger<GetAllTransactionTypeQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllTransactionTypeQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _shipperInvoiceRepository.GetAllTransactionType();
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

