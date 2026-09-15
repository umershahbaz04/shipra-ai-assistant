using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllGenderForSelection;
public class GetGenderForSelectionQueryHandler : RequestHandlerBase<GetGenderForSelectionQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;

  public GetGenderForSelectionQueryHandler(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<GetGenderForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetGenderForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var genderList = await _employeeRepository.GetAllGenderForSelection();
      genderList.Insert(0, new GenderLookup { GenderId = ApplicationConstants.DropDownPlaceHolderId, GenderName = ApplicationConstants.DropDownPlaceHolderName });
      if (genderList is not null)
      {
        serviceResult = new ServiceResultDTO(genderList);
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
