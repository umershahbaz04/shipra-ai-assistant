using System.Text;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetWayBill4X6ByOrderNo;

public class GetWayBill4X6ByOrderNoQueryHandler : RequestHandlerBase<GetWayBill4X6ByOrderNosQuery, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IOrderRepository _orderRepository;
  private readonly IBarcodeGenerate _barcodeGenerate;
  private readonly IConfigRepository _configRepository;

  public GetWayBill4X6ByOrderNoQueryHandler(IBarcodeGenerate barcodeGenerate, IConfigRepository configRepository, IWebHostEnvironment webHostEnvironment, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetWayBill4X6ByOrderNosQuery> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _orderRepository = orderRepository;
    _barcodeGenerate = barcodeGenerate;
    _configRepository = configRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetWayBill4X6ByOrderNosQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _orderRepository.GetOrderInfoByOrderNo(request.OrderNos!, _currentUser.ClientIdStr!);
      if (Enumerable.Count(result) > 0)
      {
        //var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.ShipraServiceKey);
        //if (mcconfig is null)
        //{
        //  throw new EntityNotFoundException("Mcconfig", "Shipra Service Value");
        //}

        var castedList = (IEnumerable<dynamic>)result!;

        DirectoryHelper directoryHelper = new DirectoryHelper();
        string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
        string templatePath = Path.Combine(accessUploadsFolder, $"4x6-label.html");

        #region dynamic path
        string accessUploadsFolderTemplate = Path.Combine(_webHostEnvironment.WebRootPath, $"Templates");
        templatePath = Path.Combine(accessUploadsFolderTemplate, $@"AWB\A6\4x6-label.html"); 
        #endregion

        //create directory for file upload
        string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"Output4x6-label_{Guid.NewGuid()}");
        ///test
        ///
        StringBuilder sb = new StringBuilder();
        foreach (var data in castedList)
        {
          var orderId = data.OrderId!.ToString();
          data.OrderItems = await _orderRepository.GetOrderItemsInfoByOrderId(orderId, _currentUser.ClientIdStr!);
          int count = 0;
          #region shipper ref abrcode
          string carrierBarCodeBase64 = string.Empty;

          #endregion
          string shipperrefBarCodeBase64 = string.Empty;
          if (!string.IsNullOrEmpty(data.RefNo))
          {
            shipperrefBarCodeBase64 = _barcodeGenerate.CreateBase64(data.RefNo);
          }
          ////barcode for report
          string orderBarCodeBase64 = _barcodeGenerate.CreateBase64(data.OrderNo);
          if (!string.IsNullOrEmpty(data.CarrierTrackingNo))
          {
            carrierBarCodeBase64 = _barcodeGenerate.CreateBase64(data.CarrierTrackingNo);
          }
          //// 
          if (data.OrderItems.Count == 0)
          {
            Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);
            string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, replacements, "");

            #region company logo
            var logoPath = ApplicationConstants.ShipraLogo;
            modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
            #endregion

            if (directoryHelper.CheckDirectoryExistAndCreate(outputFolder))
            {
              #region Order no  
              if (!string.IsNullOrEmpty(orderBarCodeBase64))
              {
                modifiedHtmlContent = modifiedHtmlContent.Replace("{{orderNoBarcode}}", $"data:image/png;base64,{orderBarCodeBase64}");
              }
              #endregion
              #region optional barcode

              #region carrier tracking no
              if (!string.IsNullOrEmpty(data.CarrierTrackingNo))
              {
                modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcode}}", $@"<td class=""bt bb bl br talc "" style=""text-align:left"">
                                    <img class=""p-2""
                                         height=""50"" 
                                         src=""data:image/png;base64,{carrierBarCodeBase64}"" />
                                    <div class=""text-below text-below-text-heading"">Carrier Tracking No</div>
                                    <div class=""text-below text-below-text"">{data.CarrierTrackingNo}</div>
                                </td>"); 
              }
              #endregion
              #region shipper ref number
              if (!string.IsNullOrEmpty(data.RefNo))
              {
                modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode}}", $@"<td class=""bt bb bl br talc "" >
                                    <img class=""p-2""
                                         height=""50"" 
                                         src=""data:image/png;base64,{shipperrefBarCodeBase64}"" />
                                    <div class=""text-below text-below-text-heading"">Shipper Ref</div>
                                    <div class=""text-below text-below-text"">{data.RefNo}</div>
                                </td>");

                //modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperBarcode}}", $"data:image/png;base64,{shipperrefBarCodeBase64}");
              }
              #endregion
              #endregion
             
            }

            modifiedHtmlContent = modifiedHtmlContent.Replace("{{itemCount}}", count + " / " + data.ItemsCount);
            modifiedHtmlContent = modifiedHtmlContent.Replace("{{printDate}}", DateTime.Now.ToString());

            sb.Append(modifiedHtmlContent);
            sb.Append("<div style=\"page-break-after: always;\"></div>");
          }
          else
          {
            foreach (var item in data.OrderItems)
            {
              count++;
              Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);
              string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, replacements, "");

              #region company logo
              var logoPath = ApplicationConstants.ShipraLogo;
              modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
              #endregion

              if (directoryHelper.CheckDirectoryExistAndCreate(outputFolder))
              {
                #region Order Barcode  
                if (!string.IsNullOrEmpty(orderBarCodeBase64))
                {
                  modifiedHtmlContent = modifiedHtmlContent.Replace("{{orderNoBarcode}}", $"data:image/png;base64,{orderBarCodeBase64}");
                }
                #endregion


                #region optional barcode

                #region carrier tracking no
                if (!string.IsNullOrEmpty(data.CarrierTrackingNo))
                {
                  modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcode}}", $@"<td class=""bt bb bl br talc "" style=""text-align:left"">
                                    <img class=""p-2""
                                         height=""50"" 
                                         src=""data:image/png;base64,{carrierBarCodeBase64}"" />
                                    <div class=""text-below text-below-text-heading"">Carrier Tracking No</div>
                                    <div class=""text-below text-below-text"">{data.CarrierTrackingNo}</div>
                                </td>"); 
                }
                #endregion
                #region shipper ref number
                if (!string.IsNullOrEmpty(data.RefNo))
                {
                  modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode}}", $@"<td class=""bt bb bl br talc "" >
                                    <img class=""p-2""
                                         height=""50"" 
                                         src=""data:image/png;base64,{shipperrefBarCodeBase64}"" />
                                    <div class=""text-below text-below-text-heading"">Shipper Ref</div>
                                    <div class=""text-below text-below-text"">{data.RefNo}</div>
                                </td>");
                   
                }
                #endregion
                #endregion 
              }
              if (string.IsNullOrEmpty(carrierBarCodeBase64) && string.IsNullOrEmpty(shipperrefBarCodeBase64))
              {
                modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcode}}", $@"<td class=""bt bb bl talc "" ></td>");
                modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode}}", $@"<td class=""bt bb br talc "" ></td>");
              }
              if (string.IsNullOrEmpty(carrierBarCodeBase64))
              {
                modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcode}}", ""); 
              }
              if (string.IsNullOrEmpty(shipperrefBarCodeBase64))
              { 
                modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode}}", "");
              }

              modifiedHtmlContent = modifiedHtmlContent.Replace("{{itemCount}}", count + " / " + data.ItemsCount);
              modifiedHtmlContent = modifiedHtmlContent.Replace("{{printDate}}", DateTime.Now.ToString());

              sb.Append(modifiedHtmlContent);
              sb.Append("<div style=\"page-break-after: always;\"></div>");
            }

          }
        }
        if (!string.IsNullOrEmpty(sb.ToString()))
        {
          var uniqueFileName = Guid.NewGuid().ToString();

          string pdfFilePath = Path.Combine(outputFolder, $"{uniqueFileName}.pdf");

          PdfWriter pdfWriter = new PdfWriter(pdfFilePath);
          PdfDocument pdfDoc = new PdfDocument(pdfWriter);
          using (Document document = new Document(pdfDoc, iText.Kernel.Geom.PageSize.A4))
          {
            document.SetMargins(5, 5, 5, 5);
            ConverterProperties converterProperties = new ConverterProperties();
            HtmlConverter.ConvertToPdf(sb.ToString(), pdfDoc, converterProperties);
          }
          byte[] bytes = PDFDocumentGenerator.ReadAllBytes(pdfFilePath);
          serviceResult = new ServiceResultDTO(bytes);

        }
        bool deleted = directoryHelper.DeleteDirectroy(outputFolder);
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  public static void SaveImageToFile(Stream imageStream, string filePath)
  {
    using (FileStream fileStream = File.Create(filePath))
    {
      imageStream.Seek(0, SeekOrigin.Begin); // Ensure the stream is at the beginning
      imageStream.CopyTo(fileStream);
    }
  }
  //public static void SaveImageToFile(byte[] imageData, string filePath)
  //{
  //  using (FileStream fileStream = File.Create(filePath))
  //  {
  //    fileStream.Write(imageData, 0, imageData.Length);
  //  }
  //}
  public static string ImageToBase64(string imagePath)
  {
    byte[] imageBytes = File.ReadAllBytes(imagePath);
    string base64String = Convert.ToBase64String(imageBytes);
    return base64String;
  }
}
