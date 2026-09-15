using System.Net;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UploadFileFullfilable;
public class UploadFileFullfilableCommand : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
  public int? StoreId { get; set; }
  public IFormFile? File { get; set; }
}
public class UploadFileFullfilableCommandHandler : RequestHandlerBase<UploadFileFullfilableCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderBoxRepository _orderBoxRepository;
  private readonly IProductStationRepository _productStationRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IS3Service _s3Service;
  private readonly IProductRepository _productRepository;
  private readonly IPaymentMethodLookupRepository _paymentMethodLookupRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public UploadFileFullfilableCommandHandler(IEmployeeRepository employeeRepository,IOrderBoxRepository orderBoxRepository,IProductStationRepository productStationRepository,IClientRepository clientRepository,IS3Service s3Service, IProductRepository productRepository, IPaymentMethodLookupRepository paymentMethodLookupRepository, ICountryRepository countryRepository, IOrderRepository orderRepository, IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<UploadFileFullfilableCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
    _orderBoxRepository = orderBoxRepository;
    _productStationRepository = productStationRepository;
    _clientRepository = clientRepository;
    _s3Service = s3Service;
    _productRepository = productRepository;
    _paymentMethodLookupRepository = paymentMethodLookupRepository;
    _countryRepository = countryRepository;
    _orderRepository = orderRepository;
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UploadFileFullfilableCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      IList<UDTOrderDetailSimplified>? dataList = null;

      string? uniqueFileName = null;
      if (request.File?.ContentType != null)
      {
        string fileUploadsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
        if (!Directory.Exists(fileUploadsFolder))
        { //check if the folder exists;
          Directory.CreateDirectory(fileUploadsFolder);
        }
        uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File?.FileName;
        string filePath = System.IO.Path.Combine(fileUploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          request.File?.CopyTo(fileStream);
        }
        ExcelFileProcessorByCountry excelProcessor = new ExcelFileProcessorByCountry();
        var processExcelResponseStr = excelProcessor.ProcessExcelData(request.CountryId);


        if (processExcelResponseStr == UploadExcelFileConstant.AddProductDataWithCityAndArea)
        {
          dataList = ExcelReader.GetDataToList(filePath, AddProductDataWithCityAndArea);
        }
        //else if (request.CountryId == (int)EnumCountry.SaudiArabia)
        //{
        //  dataList = ExcelReader.GetDataToList(filePath, AddProductDataWithCityAndArea);
        //}
        else if (processExcelResponseStr == UploadExcelFileConstant.AddProductDataWithStateCityPincode)
        {
          dataList = ExcelReader.GetDataToList(filePath, AddProductDataWithStateCityPincode);
        }
        else if (processExcelResponseStr == UploadExcelFileConstant.AddProductDataWithProvinceCityPincode)
        {
          dataList = ExcelReader.GetDataToList(filePath, AddProductDataWithProvinceCityPincode);
        }
        else if (processExcelResponseStr == UploadExcelFileConstant.AddProductDataWithStatCity)
        {
          dataList = ExcelReader.GetDataToList(filePath, AddProductDataWithStatCity);
        }
        if (dataList != null && dataList.Count > 0)
        {
          var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
          if (client is null)
          {
            throw new EntityNotFoundException("Client ", _currentUser.ClientId!.Value.ToString());
          }
          var s3path = ApplicationConstants.GetS3ClientFolderPattern(_currentUser.ClientId!.Value!.ToString(), ApplicationConstants.ClientUploadOrder);
          var requestResponse = await _s3Service.UploadFileAsync(request.File!, s3path);

          var countries = await _countryRepository.GetAllCountries();
          var cities = await _countryRepository.GetAllCities();
          var provinces = await _countryRepository.GetAllProvinces();
          var states = await _countryRepository.GetAllStates();
          var areas = await _countryRepository.GetAllAreas();
          var pinCodes = await _countryRepository.GetAllPinCodes();

          var paymentMethods = await _paymentMethodLookupRepository.GetAllPaymentMethodLookup();
          var allProductStations = await _productStationRepository.GetAllProductStations(_currentUser.ClientId!);
          var allOrderBox = await _orderBoxRepository.GetAllClientClientOrderBox(_currentUser.ClientId!); 
          var allEmployees = await _employeeRepository.GetAllSaleConfigEmployeesByClient(_currentUser.ClientId!);
          var allSalesPerson = await _employeeRepository.GetAllSaleChannelConfigByEmployess(_currentUser.ClientId!);

          if (request.StoreId == 0)
          {
            request.StoreId = client.DefaultStoreId;
          }
          var productStocks = await _productRepository.GetProductStocksForSelection(_currentUser.ClientId!.Value.ToString(), request.StoreId.GetValueOrDefault(), 0);

          if (_currentUser.RoleId == (int)EnumUserRole.SalePerson)
          {
            dataList.ToList().ForEach(item => item.SalePerson = string.Empty);
          }
          UDTOrderDetailResponse response = UDTOrderDetailSimplifiedFullfilable.ConvertoFullfilableOrderDetail(dataList, request.CountryId, countries, cities, provinces, states, areas, pinCodes, paymentMethods, productStocks, allProductStations,client.DefaultProductStationId.GetValueOrDefault(), allOrderBox,_mapper,allEmployees, allSalesPerson);
          var errorsList = response.Errors ?? new List<UDTFileUploadError>();
          var hasErrors = !response.IsSuccessed;

          // Extract all mobile numbers to query in a single batch
          var mobileNumbers = new List<string>();
          foreach (var x in response.UDTOrderDetail!)
          {
             if (x.OrderAddress != null)
             {
                 if (!string.IsNullOrEmpty(x.OrderAddress.Mobile1) && x.OrderAddress.Mobile1.Trim().Length >= 8)
                     mobileNumbers.Add(x.OrderAddress.Mobile1.Trim());
                 if (!string.IsNullOrEmpty(x.OrderAddress.Mobile2) && x.OrderAddress.Mobile2.Trim().Length >= 8)
                     mobileNumbers.Add(x.OrderAddress.Mobile2.Trim());
             }
          }
          mobileNumbers = mobileNumbers.Distinct().ToList();

          var bulkDuplicates = await _orderRepository.CheckMobileNosDuplicateBulk(mobileNumbers, _currentUser?.ClientIdStr ?? "");

          for (int rowIndex = 0; rowIndex < response.UDTOrderDetail!.Count; rowIndex++)
          {
            var item = response.UDTOrderDetail[rowIndex];
            item.StoreId = client!.DefaultStoreId; 
            item.OrderTypeId = (int)EnumOrderType.FullFilable;

            // Check duplicate mobile numbers using pre-fetched bulk duplicates
            if (item.OrderAddress != null)
            {
              string m1 = item.OrderAddress.Mobile1 ?? "";
              string m2 = item.OrderAddress.Mobile2 ?? "";
              
              string cleanM1 = new string(m1.Where(char.IsDigit).ToArray());
              string cleanM2 = new string(m2.Where(char.IsDigit).ToArray());

              if (cleanM1.Length >= 8 || cleanM2.Length >= 8)
              {
                var dupResult = bulkDuplicates.FirstOrDefault(d => d.IsDuplicate && d.OrderNo != null && 
                  d.MobileNo != null && 
                  ( (cleanM1.Length >= 8 && d.MobileNo.EndsWith(cleanM1.Substring(cleanM1.Length - 8))) || 
                    (cleanM2.Length >= 8 && d.MobileNo.EndsWith(cleanM2.Substring(cleanM2.Length - 8))) )
                );

                if (dupResult != null && dupResult.IsDuplicate)
                {
                  hasErrors = true;
                  var existingErr = errorsList.FirstOrDefault(x => x.Row == (rowIndex + 1));
                  if (existingErr == null)
                  {
                    existingErr = new UDTFileUploadError { Row = rowIndex + 1, IsSuccessed = false, Msg = new List<string>() };
                    errorsList.Add(existingErr);
                  }
                  existingErr.IsSuccessed = false;
                  existingErr.Msg.Add($"Duplicate Mobile! Match found in Order: {dupResult.OrderNo} ({dupResult.DaysAgo} days ago)");
                }
              }
            }
          }
          response.IsSuccessed = !hasErrors;
          response.Errors = errorsList;
          serviceResult = new ServiceResultDTO(response.UDTOrderDetail!);

          if (!response.IsSuccessed)
          {
            serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed; ;
            serviceResult.IsSuccess = false;
            var erMSg = response.Errors?.Select(x => new { x.IsSuccessed, x.Row, Msg = $"Please correct the following: " + string.Join(',', x.Msg) });
            var json = JsonConvert.SerializeObject(erMSg);
            serviceResult.Errors?.Add("InvalidParameter", new[] { json });
            serviceResult.IsSuccess = response.IsSuccessed;
          }
           
          //  file delete 
          if (System.IO.File.Exists(filePath))
          {
            System.IO.File.Delete(filePath);
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
  #region 1 
  private UDTOrderDetailSimplified AddProductDataWithCityAndArea(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTOrderDetailSimplified()
    {
      RefNo = rowData[columnNames.IndexFor("RefNo")],
      StationCode = rowData[columnNames.IndexFor("StationCode")],
      CustomerName = rowData[columnNames.IndexFor("CustomerName")],
      MobileNumber = rowData[columnNames.IndexFor("MobileNumber")],
      MobileNumber2 = rowData[columnNames.IndexFor("MobileNumber_Opt")], 
      CityName = rowData[columnNames.IndexFor("City")],
      AreaCode = rowData[columnNames.IndexFor("Area")],
      StreetAddress = rowData[columnNames.IndexFor("StreetAddress")],
      Latitude = rowData[columnNames.IndexFor("Latitude")].ToDecimal(),
      Longitude = rowData[columnNames.IndexFor("Longitude")].ToDecimal(),
      PaymentMethod = rowData[columnNames.IndexFor("PaymentMethod")],
      Amount = rowData[columnNames.IndexFor("Amount")].ToDecimal(),
      Description = rowData[columnNames.IndexFor("Description")],
      Remarks = rowData[columnNames.IndexFor("Remarks")],
      Products = rowData[columnNames.IndexFor("Products")],
      BoxName = rowData[columnNames.IndexFor("BoxName")],
      SalePerson = rowData[columnNames.IndexFor("SalePerson")],
    };
    return product;
  }
  #endregion
  #region 2 
  private UDTOrderDetailSimplified AddProductDataWithStateCityPincode(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTOrderDetailSimplified()
    {
      RefNo = rowData[columnNames.IndexFor("RefNo")],
      CustomerName = rowData[columnNames.IndexFor("CustomerName")],
      StationCode = rowData[columnNames.IndexFor("StationCode")],
      MobileNumber = rowData[columnNames.IndexFor("MobileNumber")],
      MobileNumber2 = rowData[columnNames.IndexFor("MobileNumber_Opt")], 
      State = rowData[columnNames.IndexFor("State")],
      CityName = rowData[columnNames.IndexFor("City")],
      PinCode = rowData[columnNames.IndexFor("PinCode")],
      StreetAddress = rowData[columnNames.IndexFor("StreetAddress")],
      Latitude = rowData[columnNames.IndexFor("Latitude")].ToDecimal(),
      Longitude = rowData[columnNames.IndexFor("Longitude")].ToDecimal(),
      PaymentMethod = rowData[columnNames.IndexFor("PaymentMethod")],
      Amount = rowData[columnNames.IndexFor("Amount")].ToDecimal(),
      Description = rowData[columnNames.IndexFor("Description")],
      Remarks = rowData[columnNames.IndexFor("Remarks")],
      Products = rowData[columnNames.IndexFor("Products")],
      BoxName = rowData[columnNames.IndexFor("BoxName")],
      SalePerson = rowData[columnNames.IndexFor("SalePerson")],
    };
    return product;
  }
  #endregion
  #region 4 
  private UDTOrderDetailSimplified AddProductDataWithStatCity(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTOrderDetailSimplified()
    {
      RefNo = rowData[columnNames.IndexFor("RefNo")],
      CustomerName = rowData[columnNames.IndexFor("CustomerName")],
      StationCode = rowData[columnNames.IndexFor("StationCode")],
      MobileNumber = rowData[columnNames.IndexFor("MobileNumber")],
      MobileNumber2 = rowData[columnNames.IndexFor("MobileNumber_Opt")], 
      State = rowData[columnNames.IndexFor("State")],
      CityName = rowData[columnNames.IndexFor("City")],
      //PinCode = rowData[columnNames.IndexFor("PinCode")],
      StreetAddress = rowData[columnNames.IndexFor("StreetAddress")],
      Latitude = rowData[columnNames.IndexFor("Latitude")].ToDecimal(),
      Longitude = rowData[columnNames.IndexFor("Longitude")].ToDecimal(),
      PaymentMethod = rowData[columnNames.IndexFor("PaymentMethod")],
      Amount = rowData[columnNames.IndexFor("Amount")].ToDecimal(),
      Description = rowData[columnNames.IndexFor("Description")],
      Remarks = rowData[columnNames.IndexFor("Remarks")],
      Products = rowData[columnNames.IndexFor("Products")],
      BoxName = rowData[columnNames.IndexFor("BoxName")],
      SalePerson = rowData[columnNames.IndexFor("SalePerson")],
    };
    return product;
  }
  #endregion
  #region 3 
  private UDTOrderDetailSimplified AddProductDataWithProvinceCityPincode(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTOrderDetailSimplified()
    {
      RefNo = rowData[columnNames.IndexFor("RefNo")],
      CustomerName = rowData[columnNames.IndexFor("CustomerName")],
      StationCode = rowData[columnNames.IndexFor("StationCode")],
      MobileNumber = rowData[columnNames.IndexFor("MobileNumber")],
      MobileNumber2 = rowData[columnNames.IndexFor("MobileNumber_Opt")], 
      Province = rowData[columnNames.IndexFor("Province")],
      CityName = rowData[columnNames.IndexFor("City")],
      PinCode = rowData[columnNames.IndexFor("PinCode")],
      StreetAddress = rowData[columnNames.IndexFor("StreetAddress")],
      Latitude = rowData[columnNames.IndexFor("Latitude")].ToDecimal(),
      Longitude = rowData[columnNames.IndexFor("Longitude")].ToDecimal(),
      PaymentMethod = rowData[columnNames.IndexFor("PaymentMethod")],
      Amount = rowData[columnNames.IndexFor("Amount")].ToDecimal(),
      Description = rowData[columnNames.IndexFor("Description")],
      Remarks = rowData[columnNames.IndexFor("Remarks")],
      Products = rowData[columnNames.IndexFor("Products")],
      BoxName = rowData[columnNames.IndexFor("BoxName")],
      SalePerson = rowData[columnNames.IndexFor("SalePerson")],
    };
    return product;
  }
  #endregion
  #endregion
}
