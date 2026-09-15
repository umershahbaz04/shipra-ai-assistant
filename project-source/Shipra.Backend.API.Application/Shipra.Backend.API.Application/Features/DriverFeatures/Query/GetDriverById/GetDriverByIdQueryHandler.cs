using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DriverUseCase;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetDriverById;
public class GetDriverByIdQueryHandler : RequestHandlerBase<GetDriverByIdQuery, ServiceResultDTO>
{
  private readonly IDriverRepository _driverRepository;

  public GetDriverByIdQueryHandler(IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetDriverByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDriverByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      Guid guidID;
      var hasGUID = Guid.TryParse(request!.DriverId!, out guidID);
      if (!hasGUID)
      {
        throw new InvalidIdTypeException(request!.DriverId!);
      }
      var driverId = new DriverId(new Guid(request!.DriverId!));
      var driver = await _driverRepository.GetDriverById(driverId);
      if (driver is not null)
      {
        var mappedDriver = _mapper.Map<DriverResponseModel>(driver);
        serviceResult = new ServiceResultDTO(mappedDriver);
        serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
        return serviceResult;
      }
      else
      {
        throw new EntityNotFoundException("Driver ", driverId.Value);
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
