using System.Dynamic;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarriersForSelection;
public class GetActiveCarriersForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetActiveCarriersForSelectionQueryHandler : RequestHandlerBase<GetActiveCarriersForSelectionQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetActiveCarriersForSelectionQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetActiveCarriersForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetActiveCarriersForSelectionQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var carrierList = await _carrierRepository.GetAllActiveCarriersForSelection(_currentUser.ClientId!.Value.ToString());
      var result = new ActiveCarriersForSelectionResponseModel();
      result.CarrierId = ApplicationConstants.DropDownPlaceHolderId;
      result.Name = ApplicationConstants.DropDownPlaceHolderName;
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
