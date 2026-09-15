using System.ComponentModel.DataAnnotations;
using iText.Kernel.Pdf;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAirWayBillWithDynamicTemplate;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetWayBill4X6ByOrderNo;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAwbForCarrierByOrderNos;
public class GetAwbForCarrierByOrderNosQuery : IRequest<ServiceResultDTO>
{
  [Required]
  public string? orderNos { get; set; }
}
public class GetAwbForCarrierByOrderNosQueryHandler : RequestHandlerBase<GetAwbForCarrierByOrderNosQuery, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IClientRepository _clientRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ICarrierSharedRepository _carrierSharedRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly ICarrierRepository _carrierRepository;
  private readonly IOrderRepository _orderRepository;
  public GetAwbForCarrierByOrderNosQueryHandler(IMediator mediator, IClientRepository clientRepository, IConfigRepository configRepository, ICarrierSharedRepository carrierSharedRepository, IWebHostEnvironment webHostEnvironment, ICarrierRepository carrierRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAwbForCarrierByOrderNosQueryHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _clientRepository = clientRepository;
    _configRepository = configRepository;
    _carrierSharedRepository = carrierSharedRepository;
    _webHostEnvironment = webHostEnvironment;
    _carrierRepository = carrierRepository;
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAwbForCarrierByOrderNosQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    DirectoryHelper directoryHelper = new DirectoryHelper();
    string fileUploadsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, $"ClientAwb_{_currentUser.ClientIdStr!}_{Guid.NewGuid().ToString()}");

    try
    {
     //create directory for file upload
      bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(fileUploadsFolder);

      List<Order> orderList = await _orderRepository.GetOrdersWithOrderNos(request.orderNos, _currentUser.ClientId!);
      var oClient = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (oClient is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientId!.Value.ToString());
      }
      #region in house carrier label   
      var inHouseCarrier = orderList.Where(x => x.CarrierId == oClient!.DefaultCarrierId || x.CarrierId is null).ToList();
      if (inHouseCarrier.Count > 0)
      {
        GetAirWayBillWithDynamicTemplateQuery inHouserequest = new GetAirWayBillWithDynamicTemplateQuery();
        inHouserequest.OrderNos = string.Join(',', inHouseCarrier.Select(x => x.OrderNo));
        inHouserequest.DocumentTemplateId = null;
        var inHouseResponse = await _mediator.Send(inHouserequest);

        if (inHouseResponse.IsSuccess)
        {
          // Convert byte array to MemoryStream
          try
          {
            MemoryStream oStream = new MemoryStream(inHouseResponse.Result!);
            var fileName = Guid.NewGuid().ToString();
            string filePathTemp = Path.Combine(fileUploadsFolder, fileName);
            var cPdfName = $"{filePathTemp}.pdf";
            using (var fileStream = File.Create(cPdfName))
            {
              await oStream!.CopyToAsync(fileStream);
            }
          }
          catch (Exception)
          {
          }
        }
        else
        {
          serviceResultDTO = inHouseResponse;

          return serviceResultDTO;
        }
      }
      #endregion

      #region carrier relatd awbs
      //filterd only carrier orders
      var orderCarrierGroup = orderList.Where(x => x.CarrierId != oClient!.DefaultCarrierId && x.CarrierId is not null).GroupBy(x => x.CarrierId).Select(x => new
      {
        CarrierTrackingNo = string.Join(",", orderList.Where(d => d.CarrierId == x.Key).Select(d => d.CarrierTrackingNo)),
        CarrierId = x.Key
      }).ToList();
      if (orderCarrierGroup.Count > 0)
      {
        var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);
        if (mcconfig is null)
        {
          throw new EntityNotFoundException("Mcconfig", "Integration Value");
        }
        foreach (var item in orderCarrierGroup)
        {
          var trackingNumbers = item.CarrierTrackingNo.Split(',');
          var filteredOrders = orderList.Where(order => trackingNumbers.Contains(order.CarrierTrackingNo) && order.CarrierId == item.CarrierId).ToList();
          var oOrderForIds = filteredOrders.FirstOrDefault();
          #region get data from api
          try
          {
            var uniqueFileName = Guid.NewGuid().ToString();
            string filePath = System.IO.Path.Combine(fileUploadsFolder, $"{uniqueFileName}.pdf");

            var oOrderIds = string.Join(',', filteredOrders.Select(x => x.OrderId!.Value!.ToString()));

            var carreirresult = await _carrierSharedRepository.GetCarrierAwbAsync(oOrderForIds!.CarrierId.GetValueOrDefault(), oOrderForIds!.ActiveCarrierId.GetValueOrDefault(), _currentUser.ClientIdStr!, oOrderIds, mcconfig.Value!);

            var clientId = _currentUser.ClientIdStr!;

            if (!string.IsNullOrEmpty(carreirresult))
            {
              IntegrationCarrierResponseModel<AwbListResponseModel> result = JsonConvert.DeserializeObject<IntegrationCarrierResponseModel<AwbListResponseModel>>(carreirresult);
              if (result != null && result!.isSuccess)
              {
                if (result!.data?.streams!.Count > 0)
                {
                  foreach (var oStream in result!.data?.streams!)
                  {
                    var fileName = Guid.NewGuid().ToString();
                    string filePathTemp = Path.Combine(fileUploadsFolder, fileName);
                    var cPdfName = $"{filePathTemp}.pdf";
                    using (var fileStream = File.Create(cPdfName))
                    {
                      await oStream.stream!.CopyToAsync(fileStream);
                    }
                  }
                }
              }
            }
            //Stream getData = await GetAsync(config!, item.CarrierTrackingNo!);
            //Add pages from the first document   

          }
          catch (Exception)
          {
            continue;
          }
          #endregion

        }
      }
      #endregion

      string[] filePaths = directoryHelper.GetAllPdfFiles(fileUploadsFolder);
      string opath = directoryHelper.CombinePathPdfs(fileUploadsFolder);

      bool isDone = directoryHelper.FileExistsCreateAndClose(opath);
      //mege all files
      MergePDFs(filePaths, opath);
      //files into byte

      byte[] bytes = System.IO.File.ReadAllBytes(opath);

      bool deleted = directoryHelper.DeleteDirectroy(fileUploadsFolder);

      serviceResultDTO = new ServiceResultDTO(bytes);
      serviceResultDTO.CreateSuccessResponse();
      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      //delete folder if created
      bool deleted = directoryHelper.DeleteDirectroy(fileUploadsFolder); 
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
  public void MergePDFs(string[] filepaths, string outputFilePath)
  {
    using (PdfWriter writer = new PdfWriter(outputFilePath))
    {
      using (PdfDocument mergedPdf = new PdfDocument(writer))
      {
        foreach (string filepath in filepaths)
        {
          using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(filepath)))
          {
            pdfDoc.CopyPagesTo(1, pdfDoc.GetNumberOfPages(), mergedPdf);
          }
        }
      }
    }
  }
}
