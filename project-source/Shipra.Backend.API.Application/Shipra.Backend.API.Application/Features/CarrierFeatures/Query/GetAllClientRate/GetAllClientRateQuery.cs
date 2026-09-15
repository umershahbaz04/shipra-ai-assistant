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
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllClientRate;
public class GetAllClientRateQuery :  IRequest<ServiceResultDTO>
{
  public PriceCalculatorFilter? filter { get; set; }
}
public class GetAllClientRateQueryHandler : RequestHandlerBase<GetAllClientRateQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllClientRateQueryHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<GetAllClientRateQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientRateQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {

      if (string.IsNullOrEmpty(request.filter!.ClientId))
      {
        request.filter.ClientId = _currentUser.ClientIdStr;
        var fromValue = request.filter.AddressFrom.LastOrDefault().Value?.ToString();
        var toValue = request.filter.AddressTo.LastOrDefault().Value?.ToString();

        if (long.TryParse(fromValue, out long fromLong))
        {
          request.filter.From = fromLong;
        }

        if (long.TryParse(toValue, out long toLong))
        {
          request.filter.To = toLong;
        }
      }

    
      var data = await _carrierRepository.GetAllClientRateAsync(request.filter);
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
