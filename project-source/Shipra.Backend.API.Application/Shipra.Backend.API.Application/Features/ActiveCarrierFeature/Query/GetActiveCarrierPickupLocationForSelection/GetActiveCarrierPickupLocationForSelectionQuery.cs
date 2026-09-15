using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarrierPickupLocationForSelection;
public class GetActiveCarrierPickupLocationForSelectionQuery : IRequest<ServiceResultDTO>
{
  public int? ActiveCarrierId { get; set; }
  public int? CarrierId { get; set; }
}
partial class GetActiveCarrierPickupLocationForSelectionQueryHandler : RequestHandlerBase<GetActiveCarrierPickupLocationForSelectionQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetActiveCarrierPickupLocationForSelectionQueryHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<GetActiveCarrierPickupLocationForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetActiveCarrierPickupLocationForSelectionQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var carrierList = await _carrierRepository.GetAllActiveCarrierPickupLocationByActiveCarrierIdForSelection(request.ActiveCarrierId,_currentUser.ClientIdStr!);
      var result = new ActiveCarrierPickupLocationForSelectionResponseModel();
      result.ActiveCarrierPickupLocationId = ApplicationConstants.DropDownPlaceHolderId;
      result.FullAddress = ApplicationConstants.DropDownPlaceHolderName;
      carrierList.Insert(0, result);
      if (carrierList is not null)
      {
        serviceResult = new ServiceResultDTO(carrierList);
        serviceResult.CreateSuccessResponse();
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
