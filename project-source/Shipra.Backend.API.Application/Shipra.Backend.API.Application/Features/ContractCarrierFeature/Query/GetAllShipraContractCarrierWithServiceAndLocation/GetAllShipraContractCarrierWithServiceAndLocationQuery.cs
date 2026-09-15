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
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ContractCarrierFeature.Query.GetAllShipraContractCarrierWithServiceAndLocation;
public class GetAllShipraContractCarrierWithServiceAndLocationQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int CountryId { get; set; }
  public int DeliveryServiceId { get; set; }
  public int DeliveryTypeId { get; set; } 
  public bool? IsForAdmin { get; set; }
}
public class GetAllShipraContractCarrierWithServiceAndLocationQueryHandler : RequestHandlerBase<GetAllShipraContractCarrierWithServiceAndLocationQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllShipraContractCarrierWithServiceAndLocationQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetAllShipraContractCarrierWithServiceAndLocationQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllShipraContractCarrierWithServiceAndLocationQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      ClientId? clientId = null;
      if (_currentUser != null)
      {
        clientId = _currentUser.ClientId;
      }
      var filter = request.FilterModel!;
      var shipraContract = await _carrierRepository.GetAllShipraContractCarrierWithServiceAndLocation(filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request.CountryId, request.DeliveryServiceId, request.DeliveryServiceId,request.IsForAdmin, clientId);

      serviceResult = new ServiceResultDTO(shipraContract);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
