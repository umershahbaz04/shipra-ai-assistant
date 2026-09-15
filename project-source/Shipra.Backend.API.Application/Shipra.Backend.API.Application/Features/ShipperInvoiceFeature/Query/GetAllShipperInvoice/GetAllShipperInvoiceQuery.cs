using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MediatR.Wrappers;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllShipperInvoice;
public class GetAllShipperInvoiceQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int? TransactionTypeId { get; set; }
  public int? invoiceStatusId { get; set; }
  public int? SaleChannelConfigId { get; set; }
}
public class GetAllShipperInvoiceQueryHandler : RequestHandlerBase<GetAllShipperInvoiceQuery, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public GetAllShipperInvoiceQueryHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<GetAllShipperInvoiceQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllShipperInvoiceQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var data = await _shipperInvoiceRepository.GetAllShipperInvoice(filter.CreatedFrom,filter.CreatedTo,filter.Start,filter.Length,filter.Search,filter.SortCol,filter.SortDir,_currentUser.ClientIdStr,request.TransactionTypeId,request.invoiceStatusId,request.SaleChannelConfigId);
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
