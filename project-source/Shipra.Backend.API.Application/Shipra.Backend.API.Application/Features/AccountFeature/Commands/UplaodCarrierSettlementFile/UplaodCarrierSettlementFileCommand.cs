using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder;
using Shipra.Backend.API.Core.Enum;
using Microsoft.AspNetCore.Hosting;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Application.DTOs.AccountUserCase;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.UplaodCarrierSettlementFile;
public class UplaodCarrierSettlementFileCommand : IRequest<ServiceResultDTO>
{
  public IFormFile? File { get; set; }
  public int CarrierId { get; set; }
}
public class UplaodCarrierSettlementFileCommandHandler : RequestHandlerBase<UplaodCarrierSettlementFileCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IS3Service _s3Service;
  private readonly IOrderRepository _orderRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public UplaodCarrierSettlementFileCommandHandler(IClientRepository clientRepository, IS3Service s3Service, IOrderRepository orderRepository, IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<UplaodCarrierSettlementFileCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _s3Service = s3Service;
    _orderRepository = orderRepository;
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UplaodCarrierSettlementFileCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      IList<UDTCarrierSettlementSimplified>? dataList = null;

      string? uniqueFileName = null;
      if (request.File?.ContentType != null)
      {
        string fileUploadsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
        if (!Directory.Exists(fileUploadsFolder))
        { //check if the folder exists;
          Directory.CreateDirectory(fileUploadsFolder);
        }
        uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File?.FileName;
        string filePath = Path.Combine(fileUploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          request.File?.CopyTo(fileStream);
        }
        dataList = ExcelReader.GetDataToList(filePath, AddProductData);

        if (dataList != null && dataList.Count > 0)
        {
          var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
          if (client is null)
          {
            throw new EntityNotFoundException("Client ", _currentUser.ClientId!.Value.ToString());
          }

          //var s3path = ApplicationConstants.GetS3ClientFolderPattern(_currentUser.ClientId!.Value!.ToString(), ApplicationConstants.ClientUploadOrder);
          //var requestResponse = await _s3Service.UploadFileAsync(request.File!, s3path);

          var orders = await _orderRepository.GetOrdersWithOrderNos(string.Join(',', dataList.Select(x => x.OrderNo).ToList()), _currentUser.ClientId!);

          if (orders.Count > 0)
          {
            UDTCarrierSettlementDetailResponse response = UDTCarrierSettlementSimplified.ConvertoCarrierSettlementDetail(dataList, orders,request.CarrierId);

            serviceResult = new ServiceResultDTO(response.Detail!);

            if (!response.IsSuccessed)
            {
              serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
              serviceResult.IsSuccess = false;
              var erMSg = response.Errors?.Select(x => new { x.IsSuccessed, x.Row, Msg = $"Please correct the following <br> " + string.Join(',', x.Msg) });
              var json = JsonConvert.SerializeObject(erMSg);
              serviceResult.IsSuccess = response.IsSuccessed;
              serviceResult.Errors?.Add("InvalidParameter", new[] { json });
            }
          }
          else
          {
            serviceResult.Errors!.Add("OrderNotFound", new string[] {"No Order found"});
            serviceResult.IsSuccess = false;
          }

          //  file delete 
          if (File.Exists(filePath))
          {
            // add file delete functionality
            File.Delete(filePath);
          }
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
  private UDTCarrierSettlementSimplified AddProductData(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTCarrierSettlementSimplified()
    {
      PaymentDate = rowData[columnNames.IndexFor("Date")].ToDateTime(),
      FileAmount = rowData[columnNames.IndexFor("Amount")].ToDecimal(),
      OrderNo = rowData[columnNames.IndexFor("OrderNo")],
      PaymentRef = rowData[columnNames.IndexFor("PaymentRef")],
    };
    return product;
  }
  #endregion

}
