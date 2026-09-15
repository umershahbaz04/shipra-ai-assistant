using System.Dynamic;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetDriversForSelection;
public class GetDriversForSelectionQueryHandler : RequestHandlerBase<GetDriversForSelectionQuery, ServiceResultDTO>
{
  private readonly IDriverRepository _driverRepository;
  public GetDriversForSelectionQueryHandler(IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetDriversForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverRepository = driverRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetDriversForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var driverList = await _driverRepository.GetAllDriversForSelection(_currentUser!.ClientId!);
      dynamic result = new ExpandoObject();
      result.DriverId = ApplicationConstants.DropDownPlaceHolderId;
      result.DriverName = ApplicationConstants.DropDownPlaceHolderName;
      driverList!.Insert(0, result);
      if (driverList is not null)
      {
        serviceResult = new ServiceResultDTO(driverList);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
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

