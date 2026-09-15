using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ReturnReportUseCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetOrderDetailForReturnReport;

public class GetOrderDetailForReturnReportQueryHandler : RequestHandlerBase<GetOrderDetailForReturnReportQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IOrderRepository _orderRepository;

  public GetOrderDetailForReturnReportQueryHandler(IClientRepository clientRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetOrderDetailForReturnReportQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderDetailForReturnReportQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var oOrderList = await _orderRepository.GetOrderDetailByReturnReportFile(request.OrderNo!, request.TrackingNo!, _currentUser.ClientIdStr!);     

      if (oOrderList.Count > 0)
      {
        List<UDTReturnReportSimplified> excelDataList = new List<UDTReturnReportSimplified>();
        UDTReturnReportSimplified uDTReturnReportSimplified = new UDTReturnReportSimplified()
        {
          OrderNo = request.OrderNo,
          TrackingNo = request.TrackingNo
        };
        excelDataList.Add(uDTReturnReportSimplified);

        UDTReturnReportDetailResponse response = UDTReturnReportSimplified.ConvertoReturnReportDetail(excelDataList, oOrderList, request.CarrierId);

        serviceResult = new ServiceResultDTO(response.Data!);

        if (!response.IsSuccessed)
        {
          serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
          serviceResult.IsSuccess = false;
          var erMSg = response.Errors?.Select(x => new { x.IsSuccessed, x.Row, Msg = string.Join(',', x.Msg) });
          var json = JsonConvert.SerializeObject(erMSg);
          serviceResult.IsSuccess = response.IsSuccessed;
          serviceResult.Errors?.Add("InvalidParameter", new[] { json });
        }
      }
      else
      {
        serviceResult.Errors!.Add("OrderNotFound", new string[] { "No Order found Against " + request.OrderNo });
        serviceResult.IsSuccess = false;
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
