using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDriverOrdersByDriverId;
public class GetDriverOrdersByDriverIdQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }

}
public class GetDriverOrdersByDriverIdQueryHandler : RequestHandlerBase<GetDriverOrdersByDriverIdQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IDriverRepository _driverRepository;

  public GetDriverOrdersByDriverIdQueryHandler(IDeliveryNoteRepository deliveryNoteRepository, IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetDriverOrdersByDriverIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDriverOrdersByDriverIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    BaseResponseDto baseResponse = new BaseResponseDto();
    try
    {
      var oDriver = await _driverRepository.GetDriverByEmployeeId(_currentUser.EmployeeId!);
      if (oDriver is not null)
      {
        var filter = request?.FilterModel!; 

        var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDriverOrdersByDriverId(oDriver.DriverId!.Value.ToString()!, _currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir);
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
