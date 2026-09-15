using MediatR;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ShipperInvoiceUserCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllShipperForGenerateInvocice;
public class GetAllShipperForGenerateInvociceQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllShipperForGenerateInvociceQueryHandler : RequestHandlerBase<GetAllShipperForGenerateInvociceQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetAllShipperForGenerateInvociceQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<GetAllShipperForGenerateInvociceQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllShipperForGenerateInvociceQuery request, CancellationToken cancellationToken)
  {
    var result = new ServiceResultDTO();

    try
    { 
      var data = await _shipperInvoiceRepository.GetAllShipperForGenerateInvocice(_currentUser.ClientIdStr);
       
      result = new ServiceResultDTO(data);
      return result;
    }
    catch (Exception ex)
    {
      result.CreateErrorResponse(ex);
      return result;
    }
  }
}
