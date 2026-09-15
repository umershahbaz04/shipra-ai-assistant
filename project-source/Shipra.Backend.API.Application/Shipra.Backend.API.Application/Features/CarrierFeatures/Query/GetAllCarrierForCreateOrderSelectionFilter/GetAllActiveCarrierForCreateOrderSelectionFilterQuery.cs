using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Common;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllCarrierForCreateOrderSelectionFilter;
public class GetAllActiveCarrierForCreateOrderSelectionFilterQuery : IRequest<ServiceResultDTO>
{
  public int? CountryId { get; set; }
} 
public class GetAllActiveCarrierForCreateOrderSelectionFilterQueryHandelr : RequestHandlerBase<GetAllActiveCarrierForCreateOrderSelectionFilterQuery,ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllActiveCarrierForCreateOrderSelectionFilterQueryHandelr(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<GetAllActiveCarrierForCreateOrderSelectionFilterQueryHandelr> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }
   

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllActiveCarrierForCreateOrderSelectionFilterQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    { 
      var data = await _carrierRepository.GetAllActiveCarrierForCreateOrderSelectionFilterByCountry(_currentUser.ClientIdStr,request.CountryId); 
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
