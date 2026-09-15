using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetAllDeliveryNoteForDriverById;

public class GetAllDeliveryNoteForDriverByIdQueryHandler : RequestHandlerBase<GetAllDeliveryNoteForDriverByIdQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IDriverRepository _driverRepository;

  public GetAllDeliveryNoteForDriverByIdQueryHandler(IDeliveryNoteRepository deliveryNoteRepository, IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetAllDeliveryNoteForDriverByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
    _driverRepository = driverRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDeliveryNoteForDriverByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oDriver = await _driverRepository.GetDriverByEmployeeId(_currentUser.EmployeeId!);
      if (oDriver is not null)
      {
        var oCountDeliveryNote = await _deliveryNoteRepository.GetAllDeliveryNoteForDriverById(oDriver.DriverId!.Value.ToString()!, _currentUser.ClientIdStr!);
        if (oCountDeliveryNote is not null)
        {
          serviceResult = new ServiceResultDTO(oCountDeliveryNote);
          return serviceResult;
        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Delivery Note not found");
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Driver not found");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
