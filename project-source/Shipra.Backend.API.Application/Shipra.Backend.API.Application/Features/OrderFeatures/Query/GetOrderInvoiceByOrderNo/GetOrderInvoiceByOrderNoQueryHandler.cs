using System.Text;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderInvoiceByOrderNo;
public class GetOrderInvoiceByOrderNoQueryHandler : RequestHandlerBase<GetOrderInvoiceByOrderNoQuery, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IOrderRepository _orderRepository;

  public GetOrderInvoiceByOrderNoQueryHandler(IWebHostEnvironment webHostEnvironment, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetOrderInvoiceByOrderNoQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderInvoiceByOrderNoQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _orderRepository.GetOrderInfoByOrderNo(request.OrderNos!, _currentUser.ClientIdStr!);
      if (Enumerable.Count(result) > 0)
      {
        var castedList = (IEnumerable<dynamic>)result!;
        DirectoryHelper directoryHelper = new DirectoryHelper();
        string accessUploadsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
        string templatePath = System.IO.Path.Combine(accessUploadsFolder, $"orderinvoice.html");

        //create directory for file upload
        string outputFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, $"Outputorderinvoice_{_currentUser.ClientIdStr!}");
        bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(outputFolder);

        StringBuilder sb = new StringBuilder();

        foreach (var data in castedList)
        {
          var orderId = data.OrderId!.ToString();
          data.OrderItems = await _orderRepository.GetOrderItemsInfoByOrderId(orderId, _currentUser.ClientIdStr!);

          Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);
           

          StringBuilder sbItems = new StringBuilder();
          decimal total = 0;
          var orderItems = ((IEnumerable<dynamic>)data.OrderItems).ToList();

          foreach (dynamic item in orderItems)
          {

            Dictionary<string, object> listItemsDic = PDFDocumentGenerator.ConvertDynamicToDictionary(item);

            bool isSingleItem = orderItems.Count == 1;

            var description = isSingleItem
                ? data.Description
                : Utils.GetValueFromDictionryByKey("orderItemDescription", listItemsDic);

            var price = isSingleItem
                ? data.Amount
                : Utils.GetValueFromDictionryByKey("price", listItemsDic);


            var oitem = $@"<tr>
                          <td>{Utils.GetValueFromDictionryByKey("productStockSku", listItemsDic)}</td>
                        <td>{(!string.IsNullOrEmpty(description) ? description : "")}</td>
                        <td>{Utils.GetValueFromDictionryByKey("orderItemQuantity", listItemsDic)}</td>
                        <td>{price}</td>
                         <td>{Utils.GetValueFromDictionryByKey("orderItemDiscount", listItemsDic)}</td>
                      
                     
                    </tr>";
            sbItems.Append(oitem);
          }
          string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, replacements, sbItems.ToString());
          var header = @"<tr>
                        <th>Sku</th>
                        <th>Description</th>
                        <th>Quantity</th>
                        <th>Price</th>
                        <th>Discount</th>
                    </tr>";
          var subTotal = data.Amount - data.TotalTax;
          total = subTotal + data.TotalTax - data.Discount;

          modifiedHtmlContent = modifiedHtmlContent.Replace("{{orderitemHeader}}", header);
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{subTotal}}", subTotal.ToString());
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{total}}", total.ToString());
          #region store
          var store = data.StoreName;
          if (!string.IsNullOrEmpty(data.StoreImage))
          {
            store = @$"<img src=""{data.StoreImage}"" style=""height:50px;"" />";
          }
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{store}}", store); 
          #endregion
          sb.Append(modifiedHtmlContent);
        }

        if (!string.IsNullOrEmpty(sb.ToString()))
        {
          var uniqueFileName = Guid.NewGuid().ToString();
          string pdfFilePath = System.IO.Path.Combine(outputFolder, $"{uniqueFileName}.pdf");

          PdfWriter pdfWriter = new PdfWriter(pdfFilePath);
          PdfDocument pdfDoc = new PdfDocument(pdfWriter);
          using (Document document = new Document(pdfDoc, iText.Kernel.Geom.PageSize.A4))
          {
            document.SetMargins(20, 20, 40, 40);
            ConverterProperties converterProperties = new ConverterProperties();
            HtmlConverter.ConvertToPdf(sb.ToString(), pdfDoc, converterProperties);
          }
          byte[] bytes = PDFDocumentGenerator.ReadAllBytes(pdfFilePath);
          serviceResult = new ServiceResultDTO(bytes);

        }

        // byte[] bytes = PDFDocumentGenerator.ReadAllBytes(pdfFilePath);

        bool deleted = directoryHelper.DeleteDirectroy(outputFolder);

        //serviceResult = new ServiceResultDTO(bytes);
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
