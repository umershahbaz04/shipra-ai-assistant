using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetAllDrivers;
public class GetAllDriversQueryHandler : RequestHandlerBase<GetAllDriversQuery, ServiceResultDTO>
{
  private readonly IDriverRepository _driverRepository;

  public GetAllDriversQueryHandler(IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetAllDriversQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDriversQuery request, CancellationToken cancellationToken)
  {
    var serviceResultDTO = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      dynamic oDrivers = await _driverRepository.GetAllDrivers(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, _currentUser.ClientId!.Value.ToString());

      serviceResultDTO = new ServiceResultDTO(oDrivers); 
      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
