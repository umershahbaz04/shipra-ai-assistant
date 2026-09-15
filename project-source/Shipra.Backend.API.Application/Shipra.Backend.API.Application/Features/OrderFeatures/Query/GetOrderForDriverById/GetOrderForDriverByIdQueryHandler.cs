using System.Dynamic;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderForDriverById;
public class GetOrderForDriverByIdQueryHandler : RequestHandlerBase<GetOrderForDriverByIdQuery, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDriverRepository _driverRepository;

  public GetOrderForDriverByIdQueryHandler(IStoreRepository storeRepository, IOrderRepository orderRepository, IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetOrderForDriverByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
    _orderRepository = orderRepository;
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderForDriverByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    BaseResponseDto baseResponse = new BaseResponseDto();
    try
    {
      var oDriver = await _driverRepository.GetDriverByEmployeeId(_currentUser.EmployeeId!);
      if (oDriver is not null)
      {
        var oOrder = await _orderRepository.GetOrderForDriverById(request.OrderId, oDriver.DriverId!.Value.ToString()!,_currentUser.ClientIdStr!);
        if (oOrder is not null)
        {
          var oOrderAddress = await _orderRepository.GetOrderAddressById(oOrder.OrderAddressId);
          var oFromAddress = await _storeRepository.GetStoreAddressByStoreId(oOrder.StoreId, _currentUser.ClientIdStr!);

          dynamic result = new ExpandoObject();
          result.OrderInfo = oOrder;
          result.To = oOrderAddress;
          result.From = oFromAddress;
          if (result is not null)
          {
            serviceResult = new ServiceResultDTO(result);
            serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
          }
          else
          {
            baseResponse = new BaseResponseDto
            {
              Data = oDriver.DriverId!.Value.ToString()!,
              Message = "Order not found for this driver "
            };
            serviceResult = new ServiceResultDTO(baseResponse);
          }
        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Order not found");
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Driver not found");
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
