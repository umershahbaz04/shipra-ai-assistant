using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.UploadStore;
public class UploadStoreCommand : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
  public IFormFile? File { get; set; }
}
public class UploadStoreCommandHandler : RequestHandlerBase<UploadStoreCommand, ServiceResultDTO>
{
  private readonly ICatalogueRepository _catalogueRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IStoreRepository _storeRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IClientRepository _clientRepository;

  public UploadStoreCommandHandler(ICatalogueRepository catalogueRepository,ICountryRepository countryRepository,IWebHostEnvironment webHostEnvironment, IStoreRepository storeRepository, IEmployeeRepository employeeRepository, IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<UploadStoreCommandHandler> logger) : base(serviceProvider, logger)
  {
    _catalogueRepository = catalogueRepository;
    _countryRepository = countryRepository;
    _webHostEnvironment = webHostEnvironment;
    _storeRepository = storeRepository;
    _employeeRepository = employeeRepository;
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UploadStoreCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      IList<UDTStoreDetailSimplified>? dataList = null;

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

          //var s3path = ApplicationConstants.GetS3ClientFolderPattern(_currentUser.ClientId!.Value!.ToString(), ApplicationConstants.ClientUploadOrder);
          //var requestResponse = await _s3Service.UploadFileAsync(request.File!, s3path);

          var countries = await _countryRepository.GetAllCountries();
          var cities = await _countryRepository.GetAllCities();
          var provinces = await _countryRepository.GetAllProvinces();
          var states = await _countryRepository.GetAllStates();
          var areas = await _countryRepository.GetAllAreas();
          var pinCodes = await _countryRepository.GetAllPinCodes();
           
          var allStores = await _storeRepository.GetAllStoresByClient(_currentUser.ClientId!);
          var allEmployees = await _employeeRepository.GetAllSaleConfigEmployeesByClient(_currentUser.ClientId!); 
          var oCatalouge = await _catalogueRepository.GetCatalogByClientId(_currentUser.ClientIdStr!); 

         
          UDTStoreDetailResponse response = UDTStoreDetailSimplified.ConvertoStoreDetail(dataList, request.CountryId, countries, cities, provinces, states, areas, pinCodes,oCatalouge,allStores!,allEmployees,client);
          
          serviceResult = new ServiceResultDTO(response.UDTStoreDetail!);

          if (!response.IsSuccessed)
          {
            serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed; ;
            serviceResult.IsSuccess = false;
            var erMSg = response.Errors?.Select(x => new { x.IsSuccessed, x.Row, Msg = $"Please correct the following: " + string.Join(',', x.Msg) });
            var json = JsonConvert.SerializeObject(erMSg);
            serviceResult.IsSuccess = response.IsSuccessed;
            serviceResult.Errors?.Add("InvalidParameter", new[] { json });
          }

          //  file delete 
          if (System.IO.File.Exists(filePath))
          {
            System.IO.File.Delete(filePath);
          }
          // add file delete functionality
        }
      }


      await Task.Delay(1);

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
  private UDTStoreDetailSimplified AddProductDataWithCityAndArea(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTStoreDetailSimplified()
    {
      StoreName = rowData[columnNames.IndexFor("StoreName")],
      CompanyName = rowData[columnNames.IndexFor("CompanyName")],
      CustomerServiceNo = rowData[columnNames.IndexFor("CustomerServiceNo")], 
      Phone = rowData[columnNames.IndexFor("PhoneNo_Opt")],
      CityName = rowData[columnNames.IndexFor("City")],
      AreaCode = rowData[columnNames.IndexFor("Area")],
      StreetAddress = rowData[columnNames.IndexFor("StreetAddress")],
      Latitude = rowData[columnNames.IndexFor("Latitude")].ToDecimal(),
      Longitude = rowData[columnNames.IndexFor("Longitude")].ToDecimal(), 
      DateOfBirth = rowData[columnNames.IndexFor("DOB")].ToDateTime(),
      Password = rowData[columnNames.IndexFor("Password")] 
    };
    return product;
  }
  #endregion
  #region 2 
  private UDTStoreDetailSimplified AddProductDataWithStateCityPincode(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTStoreDetailSimplified()
    {
      StoreName = rowData[columnNames.IndexFor("StoreName")],
      CompanyName = rowData[columnNames.IndexFor("CompanyName")],
      CustomerServiceNo = rowData[columnNames.IndexFor("CustomerServiceNo")],
      Phone = rowData[columnNames.IndexFor("PhoneNo_Opt")],
      State = rowData[columnNames.IndexFor("State")],
      CityName = rowData[columnNames.IndexFor("City")],
      PinCode = rowData[columnNames.IndexFor("PinCode")],
      StreetAddress = rowData[columnNames.IndexFor("StreetAddress")],
      Latitude = rowData[columnNames.IndexFor("Latitude")].ToDecimal(),
      Longitude = rowData[columnNames.IndexFor("Longitude")].ToDecimal(),
      DateOfBirth = rowData[columnNames.IndexFor("DOB")].ToDateTime(),
      Password = rowData[columnNames.IndexFor("Password")]
    };
    return product;
  }
  #endregion  
  #region 4 
  private UDTStoreDetailSimplified AddProductDataWithStatCity(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTStoreDetailSimplified()
    {
      StoreName = rowData[columnNames.IndexFor("StoreName")],
      CompanyName = rowData[columnNames.IndexFor("CompanyName")],
      CustomerServiceNo = rowData[columnNames.IndexFor("CustomerServiceNo")],
      Phone = rowData[columnNames.IndexFor("PhoneNo_Opt")],
      State = rowData[columnNames.IndexFor("State")],
      CityName = rowData[columnNames.IndexFor("City")],
      //PinCode = rowData[columnNames.IndexFor("PinCode")],
      StreetAddress = rowData[columnNames.IndexFor("StreetAddress")],
      Latitude = rowData[columnNames.IndexFor("Latitude")].ToDecimal(),
      Longitude = rowData[columnNames.IndexFor("Longitude")].ToDecimal(),
      DateOfBirth = rowData[columnNames.IndexFor("DOB")].ToDateTime(),
      Password = rowData[columnNames.IndexFor("Password")]
    };
    return product;
  }
  #endregion
  #region 3 
  private UDTStoreDetailSimplified AddProductDataWithProvinceCityPincode(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTStoreDetailSimplified()
    {
      StoreName = rowData[columnNames.IndexFor("StoreName")],
      CompanyName = rowData[columnNames.IndexFor("CompanyName")],
      CustomerServiceNo = rowData[columnNames.IndexFor("CustomerServiceNo")],
      Phone = rowData[columnNames.IndexFor("PhoneNo_Opt")],
      Province = rowData[columnNames.IndexFor("Province")],
      CityName = rowData[columnNames.IndexFor("City")],
      PinCode = rowData[columnNames.IndexFor("PinCode")],
      StreetAddress = rowData[columnNames.IndexFor("StreetAddress")],
      Latitude = rowData[columnNames.IndexFor("Latitude")].ToDecimal(),
      Longitude = rowData[columnNames.IndexFor("Longitude")].ToDecimal(),
      DateOfBirth = rowData[columnNames.IndexFor("DOB")].ToDateTime(),
      Password = rowData[columnNames.IndexFor("Password")]
    };
    return product;
  }
  #endregion
  #endregion

}
public class UploadStoreCommandValidator : AbstractValidator<UploadStoreCommand>
{
  public UploadStoreCommandValidator()
  {

  }
}
