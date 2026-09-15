using System.Text;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ExpenseFeatures.Query.GetPDFDriverExpenseReport;
public class GetPDFDriverExpenseReportQueryHandler : RequestHandlerBase<GetPDFDriverExpenseReportQuery, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IDriverAccountRepository _expenseRepository;

  public GetPDFDriverExpenseReportQueryHandler(IConfigRepository configRepository, IWebHostEnvironment webHostEnvironment, IDriverAccountRepository expenseRepository, IServiceProvider serviceProvider, ILogger<GetPDFDriverExpenseReportQueryHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _webHostEnvironment = webHostEnvironment;
    _expenseRepository = expenseRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPDFDriverExpenseReportQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var oExpenseList = await _expenseRepository.GetAllDriverExpense(_currentUser.ClientId!.Value.ToString(), filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request.DriverId!, request.ExpenseCategoryId!);
     
        var castedList = (IEnumerable<dynamic>)oExpenseList!.list;
        var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.ShipraServiceKey, _currentUser.EnvironmentTypeId);
        if (mcconfig is null)
        {
          throw new EntityNotFoundException("Mcconfig", "Shipra Service Value");
        }
        DirectoryHelper directoryHelper = new DirectoryHelper();
        string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
        string templatePath = Path.Combine(accessUploadsFolder, $"expensereport.html");
        var totalAmount = 0;
        //create directory for file upload
        string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"ExpenseReport_{Guid.NewGuid()}");
        directoryHelper.CheckDirectoryExistAndCreate(outputFolder);
        StringBuilder sb = new StringBuilder();
        StringBuilder sbItems = new StringBuilder();
        Dictionary<string, object> replacements = new Dictionary<string, object>();
        int serialNo = 0;
        foreach (var item in oExpenseList!.list)
        {
          serialNo++;
          replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(item);
          totalAmount += item.Amount;
          var date = "";
          if (replacements.ContainsKey("expenseDate"))
          {
            DateTime.TryParse(Utils.GetValueFromDictionryByKey("expenseDate", replacements!), out DateTime result);
            date = result.ToString("dd-MM-yyyy");
          }

          var oitem = $@"<tr>
                          <td  class='bl bb'> {serialNo}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("driverName", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("driverCode", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("noteNo", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("expenseCategoryName", replacements)}</td>
                          <td  class='bl bb'>{date!}</td>
                          <td  class='bl bb br'>{Utils.GetValueFromDictionryByKey("amount", replacements)}</td>          
            </tr>";
          sbItems.Append(oitem);

        }

        string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, replacements, "");

        #region company logo
        var logoPath = ApplicationConstants.ShipraLogo;
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
        if (castedList.Count() > 0)
        {
          var obj = castedList.FirstOrDefault();
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{clientName}}", obj?.ClientName);
        }
        #endregion
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportDatas}}", sbItems.ToString());
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{date}}", DateTime.Now.ToString("dd-MM-yyyy"));
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipmentCount}}", serialNo.ToString());
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{totalAmount}}", totalAmount.ToString());

        sb.Append(modifiedHtmlContent);
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
      
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
