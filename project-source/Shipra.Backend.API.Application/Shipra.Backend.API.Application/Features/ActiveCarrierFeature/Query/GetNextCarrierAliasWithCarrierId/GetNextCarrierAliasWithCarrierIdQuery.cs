using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetNextCarrierAliasWithCarrierId;
public class GetNextCarrierAliasWithCarrierIdQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
}
public class GetNextCarrierAliasWithCarrierIdQueryHandler : RequestHandlerBase<GetNextCarrierAliasWithCarrierIdQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetNextCarrierAliasWithCarrierIdQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetNextCarrierAliasWithCarrierIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetNextCarrierAliasWithCarrierIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oCarrier = await _carrierRepository.GetCarrierById(request.CarrierId);
      if (oCarrier is not null)
      {
        List<ActiveCarrier> oActiveCarrierAlias = await _carrierRepository.GetActiveCarrieriersByCarrierId(request.CarrierId, _currentUser.ClientId);
        int count = oActiveCarrierAlias.Count + 1;
        var name = oCarrier.Name + " " + count;

        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = name,
          Message = name,
        });
      } 
      else
      {
        serviceResult.CreateError("NotFounc", new string[] { "Carrier No Found." });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
