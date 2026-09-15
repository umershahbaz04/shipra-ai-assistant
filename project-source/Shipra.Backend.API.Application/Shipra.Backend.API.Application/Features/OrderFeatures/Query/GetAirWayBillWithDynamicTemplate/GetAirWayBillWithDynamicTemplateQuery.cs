using System.Dynamic;
using System.Globalization;
using System.Text;
using FluentValidation;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DocumentGenerator.PDFReports;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAirWayBillWithDynamicTemplate;
public class GetAirWayBillWithDynamicTemplateQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
  public int? DocumentTemplateId { get; set; }
  // client id must when we dont have aceess token. this request also used for front facing website
  public string? ClientId { get; set; }
}
public class GetAirWayBillWithDynamicTemplateQueryHandler : RequestHandlerBase<GetAirWayBillWithDynamicTemplateQuery, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IOrderRepository _orderRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly ISharedBarcodeRepository _sharedBarcodeRepository;
  private readonly IDocumentRepository _documentRepository;
  private readonly IBarcodeGenerate _barcodeGenerate;
  private readonly IConfigRepository _configRepository;
  public GetAirWayBillWithDynamicTemplateQueryHandler(IStoreRepository storeRepository,ICountryRepository countryRepository, ISharedBarcodeRepository sharedBarcodeRepository, IDocumentRepository documentRepository, IBarcodeGenerate barcodeGenerate, IConfigRepository configRepository, IWebHostEnvironment webHostEnvironment, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAirWayBillWithDynamicTemplateQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _orderRepository = orderRepository;
    _storeRepository = storeRepository;
    _countryRepository = countryRepository;
    _sharedBarcodeRepository = sharedBarcodeRepository;
    _documentRepository = documentRepository;
    _barcodeGenerate = barcodeGenerate;
    _configRepository = configRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAirWayBillWithDynamicTemplateQuery request, CancellationToken cancellationToken)
  {

    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      _currentUser.ClientIdStr = !string.IsNullOrEmpty(request.ClientId) ? request.ClientId : _currentUser.ClientIdStr;
      if (string.IsNullOrEmpty(_currentUser.ClientIdStr))
      {
        serviceResult.CreateError("ClientIdNotFound", new string[] { "Client is required" });
        return serviceResult;
      }
      if (!GuidHelper.Validator(_currentUser.ClientIdStr))
      {
        serviceResult.CreateError("Invalid", new string[] { GuidHelper.GuidMessage });
        return serviceResult;
      }
      _currentUser.ClientId = new ClientId(new Guid(_currentUser.ClientIdStr));
      if (request.DocumentTemplateId == null || request.DocumentTemplateId == 0)
      {
        // get default client config
        var oDocumentTemplateConfig = await _documentRepository.GetDocumentTemplateConfigByClientId(_currentUser.ClientId!);
        //set again tempate id
        if (oDocumentTemplateConfig is not null)
        {
          request.DocumentTemplateId = oDocumentTemplateConfig.DocumentTemplateId;
        }
        else
        {
          request.DocumentTemplateId = (int)EnumDocumentTemplate.Design1A4; 
        }
      }
      var oDocumentTemplate = await _documentRepository.GetDocumentTemplateById(request.DocumentTemplateId, _currentUser.ClientId!);

      if (oDocumentTemplate is null)
      {
        serviceResult.CreateError("NoDocTemplateSettingFound", new string[] { "No Document template setting found please contact with administration" });
        return serviceResult;
      }

      var documentTemplatePath = !string.IsNullOrEmpty(oDocumentTemplate.Path) ? oDocumentTemplate.Path : $@"AWB\A6\4x6-label.html";

      #region dynamic path
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"Templates");
      string templatePath = Path.Combine(accessUploadsFolder, documentTemplatePath);
      #endregion

      #region awb 
      var result = await _orderRepository.GetOrderInfoByOrderNo(request.OrderNos!, _currentUser.ClientIdStr!);
      if (Enumerable.Count(result) > 0)
      {
        var castedList = (IEnumerable<dynamic>)result!;
        if (oDocumentTemplate.DocumentTemplateId != (int)EnumDocumentTemplate.Design2A5)
        {
          var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.ShipraServiceKey, _currentUser.EnvironmentTypeId);
          if (mcconfig is null)
          {
            throw new EntityNotFoundException("Mcconfig", "Service Value");
          }

          Stream? pdfData = await GetAwbDataFromService(oDocumentTemplate.DocumentTemplateId, result, mcconfig.Value!);
          byte[] bytes = PDFDocumentGenerator.ConvertStreamToByteArray(pdfData);
          serviceResult = new ServiceResultDTO(bytes);

          return serviceResult;
        }

        #region MyRegion
        DirectoryHelper directoryHelper = new DirectoryHelper();
        //string templatePath = Path.Combine(accessUploadsFolder, $"4x6-label.html");
        //create directory for file upload
        string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"Awb_label_{(int)EnumDocumentType.AWB}_{Guid.NewGuid()}");
        ///test
        ///
        StringBuilder sb = new StringBuilder();
        foreach (var data in castedList)
        {
          var orderId = data.OrderId!.ToString();
          data.OrderItems = await _orderRepository.GetOrderItemsInfoByOrderId(orderId, _currentUser.ClientIdStr!);
          AirwayBillGenerator airwayBillGenerator = new AirwayBillGenerator(_barcodeGenerate);


          StringBuilder awgResult = airwayBillGenerator.GetAwbModifiedHtml(data, outputFolder, templatePath, directoryHelper);

          sb.Append(awgResult.ToString());
        }


        if (!string.IsNullOrEmpty(sb.ToString()))
        {
          var uniqueFileName = Guid.NewGuid().ToString();

          string pdfFilePath = Path.Combine(outputFolder, $"{uniqueFileName}.pdf");

          PdfWriter pdfWriter = new PdfWriter(pdfFilePath);
          PdfDocument pdfDoc = new PdfDocument(pdfWriter);
          //using (Document document = new Document(pdfDoc, new iText.Kernel.Geom.PageSize(288, 432)))
          using (Document document = new Document(pdfDoc, iText.Kernel.Geom.PageSize.A4)) // make this dynanic
          {
            //document.SetMargins(5, 5, 5, 5);
            ConverterProperties converterProperties = new ConverterProperties();
            HtmlConverter.ConvertToPdf(sb.ToString(), pdfDoc, converterProperties);
          }
          byte[] bytes = PDFDocumentGenerator.ReadAllBytes(pdfFilePath);
          serviceResult = new ServiceResultDTO(bytes);

        }
        bool deleted = directoryHelper.DeleteDirectroy(outputFolder);
        #endregion
      }

      #endregion
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<Stream> GetAwbDataFromService(int documentTemplateId,dynamic result,string domainUrl)
  {
    var countries = await _countryRepository.GetAllCountries(); 
    var wayBillList = new List<dynamic>();

    foreach (var data in result)
    {
      var orderId = data.OrderId!.ToString();

      // Prepare data
      var orderItems = await _orderRepository.GetOrderItemsInfoByOrderId(orderId, _currentUser.ClientIdStr!);

      // Create dynamic object
      dynamic wayBillWrapper = new ExpandoObject();
      wayBillWrapper.AirwayBillData = data;
      wayBillWrapper.OrderItems = orderItems;
      wayBillWrapper.PickupRequest = null;

      data.Store = new { label = "", text = "" };
      data.Customer = new { label = "", text = "" };
       

      #region for address city
      var country = countries.FirstOrDefault(x => x.Name?.Trim() == data?.ConsigneCountryName);
      if (country is not null)
      {
        var reqKey = Utility.GetFirstRequiredFieldKey(country.AddressingScheme!);

        if (reqKey != null)
        {
          OrderAddress orderAddress = await _orderRepository.GetOrderAddressById(data.OrderAddressId);
          if (orderAddress is not null)
          {
            int columnValue = UtilityHelper.GetColumnValueByKey(orderAddress, reqKey);
            var entityName = await _countryRepository.GetAddressEntityNameByKey(reqKey, columnValue);

            string pascalKey = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reqKey!.ToLower());

            data.Customer = new { label = pascalKey, text = entityName };

          }
        }

      } 
      #endregion 
      #region for store city
      var countryStore = countries.FirstOrDefault(x => x.Name?.Trim() == data?.StoreCountry);
      if (countryStore is not null)
      {
        var reqKey = Utility.GetFirstRequiredFieldKey(countryStore.AddressingScheme!);

        if (reqKey != null)
        {
          StoreAddress storeAddress = await _storeRepository.GetStoreAddressById(data.StoreId);
          if (storeAddress is not null)
          {
            int columnValue = UtilityHelper.GetColumnValueByKey(storeAddress, reqKey);
            var entityName = await _countryRepository.GetAddressEntityNameByKey(reqKey, columnValue);

            string pascalKey = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reqKey!.ToLower());

            data.Store = new { label = pascalKey, text = entityName }; 
          }
        }

      } 
      #endregion
      wayBillList.Add(wayBillWrapper);
    }

    var jsonData = JsonConvert.SerializeObject(new { documentTemplateId = documentTemplateId , list = wayBillList }, Formatting.Indented);
    var stream = await _sharedBarcodeRepository.GetAwb(jsonData, domainUrl);

    return stream;
  }
}
public class GetAirWayBillWithDynamicTemplateQueryValidator : AbstractValidator<GetAirWayBillWithDynamicTemplateQuery>
{
  public GetAirWayBillWithDynamicTemplateQueryValidator()
  {
    RuleFor(x => x.OrderNos).NotEmpty().NotNull();
  }
}
