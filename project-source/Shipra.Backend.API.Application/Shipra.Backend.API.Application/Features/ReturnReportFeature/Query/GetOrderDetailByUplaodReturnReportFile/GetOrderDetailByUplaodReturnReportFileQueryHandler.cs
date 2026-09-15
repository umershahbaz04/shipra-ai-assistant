using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ReturnReportUseCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetOrderDetailByUplaodReturnReportFile;

public class GetOrderDetailByUplaodReturnReportFileQueryHandler : RequestHandlerBase<GetOrderDetailByUplaodReturnReportFileQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public GetOrderDetailByUplaodReturnReportFileQueryHandler(IClientRepository clientRepository, IOrderRepository orderRepository, IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<GetOrderDetailByUplaodReturnReportFileQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _orderRepository = orderRepository;
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderDetailByUplaodReturnReportFileQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      IList<UDTReturnReportSimplified>? excelDataList = null;

      string? uniqueFileName = null;
      if (request.File?.ContentType != null)
      {
        var fileUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
        if (!Directory.Exists(fileUploadsFolder))
        { //check if the folder exists;
          Directory.CreateDirectory(fileUploadsFolder);
        }
        uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File?.FileName;
        var filePath = Path.Combine(fileUploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          request.File?.CopyTo(fileStream);
        }
        excelDataList = ExcelReader.GetDataToList(filePath, AddProductData);

        if (excelDataList != null && excelDataList.Count > 0)
        {
          var oOrderList = await _orderRepository.GetOrderDetailByReturnReportFile(string.Join(',', excelDataList.Select(x => x.OrderNo).ToList()), string.Join(',', excelDataList.Select(x => x.TrackingNo).ToList()), _currentUser.ClientIdStr!);
          if (oOrderList.Count > 0)
          {
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
            serviceResult.Errors!.Add("OrderNotFound", new string[] { "No Order found" });
            serviceResult.IsSuccess = false;
          }

          //  file delete 
          if (File.Exists(filePath))
          {
            File.Delete(filePath);
          }
          // add file delete functionality
        }
      }

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  #region excel file col reader  
  private UDTReturnReportSimplified AddProductData(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTReturnReportSimplified()
    {
      OrderNo = rowData[columnNames.IndexFor("OrderNo")],
      TrackingNo = rowData[columnNames.IndexFor("TrackingNo")],
    };
    return product;
  }
  #endregion
}
