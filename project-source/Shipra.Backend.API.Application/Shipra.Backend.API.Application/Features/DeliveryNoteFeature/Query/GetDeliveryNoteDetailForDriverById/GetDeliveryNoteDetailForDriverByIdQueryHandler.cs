using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteDetailForDriverById;
public class GetDeliveryNoteDetailForDriverByIdQueryHandler : RequestHandlerBase<GetDeliveryNoteDetailForDriverByIdQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IDriverRepository _driverRepository;

  public GetDeliveryNoteDetailForDriverByIdQueryHandler(IDeliveryNoteRepository deliveryNoteRepository, IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetDeliveryNoteDetailForDriverByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDeliveryNoteDetailForDriverByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    BaseResponseDto baseResponse = new BaseResponseDto();
    try
    {
      var oDriver = await _driverRepository.GetDriverByEmployeeId(_currentUser.EmployeeId!);
      if (oDriver is not null)
      {
        var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailForDriverById(oDriver.DriverId!.Value.ToString()!, request.DeliveryNoteId,_currentUser.ClientIdStr!);
        if (oDeliveryNoteDetail is not null)
        {
          serviceResult = new ServiceResultDTO(oDeliveryNoteDetail);
          serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        }
        else
        {
          baseResponse = new BaseResponseDto
          {
            Data = oDriver.DriverId!.Value.ToString()!,
            Message = "Delivery note note found for this driver "
          };
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Driver not found");
      }
      return serviceResult!;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
