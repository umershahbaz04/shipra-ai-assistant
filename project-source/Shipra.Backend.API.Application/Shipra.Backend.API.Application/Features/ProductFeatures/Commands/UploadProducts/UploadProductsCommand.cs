using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UploadFileRegular;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using System.Net;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UploadProductRegular;
public class UploadProductCommand : IRequest<ServiceResultDTO>
{
   public IFormFile? File { get; set; }
}
public class UploadProductCommandHandler : RequestHandlerBase<UploadProductCommand, ServiceResultDTO>
{
  private readonly IProductCategoryRepository _productCategoryRepository;
  private readonly ICommonLookupRepository _commonLookupRepository;
  private readonly IProductStationRepository _productStationRepository;
  private readonly IStoreRepository _StoreRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IPaymentMethodLookupRepository _paymentMethodLookupRepository;
  private readonly IS3Service _s3Service;
  private readonly ICountryRepository _countryRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;
 
  public UploadProductCommandHandler(IProductCategoryRepository productCategoryRepository, ICommonLookupRepository commonLookupRepository, IProductStationRepository productStationRepository, IStoreRepository StoreRepository, IClientRepository clientRepository, IPaymentMethodLookupRepository paymentMethodLookupRepository, IS3Service s3Service, ICountryRepository countryRepository, IOrderRepository orderRepository, IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<UploadProductCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productCategoryRepository = productCategoryRepository;
    _commonLookupRepository = commonLookupRepository;
    _productStationRepository = productStationRepository;
    _StoreRepository = StoreRepository;
    _clientRepository = clientRepository;
    _paymentMethodLookupRepository = paymentMethodLookupRepository;
    _s3Service = s3Service;
    _countryRepository = countryRepository;
    _orderRepository = orderRepository;
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UploadProductCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      IList<UDTproductDetailSimplified>? dataList = null;

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
        

          dataList = ExcelReader.GetDataToList(filePath, AddProductData);
        
        
        if (dataList != null && dataList.Count > 0)
        {
          var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
          if (client is null)
          {
            throw new EntityNotFoundException("Client ", _currentUser.ClientId!.Value.ToString());
          }

          var s3path = ApplicationConstants.GetS3ClientFolderPattern(_currentUser.ClientId!.Value!.ToString(), ApplicationConstants.ClientUploadOrder);
          var requestResponse = await _s3Service.UploadFileAsync(request.File!, s3path);
          var productOptionLookups = await _commonLookupRepository.GetAllProductOptionLookup();
          var allProductCategorys = await _productCategoryRepository.GetAllProductCategoryLookupByClientId(_currentUser.ClientId);
          var allstores = await _StoreRepository.GetAllStoreForid(_currentUser.ClientId);
          //Category check here .....
          foreach (var product in dataList)
          {
            //Check if the category already exists
            var existingCategory = allProductCategorys?.FirstOrDefault(c => c.CategoryName!.Trim().ToLower() == product.CategoryName!.Trim().ToLower());
            if (existingCategory == null)
            {
              //Category doesn't exist, so we save it as a new category
              var newCategory = new ProductCategory
              {
                CategoryName = product.CategoryName,
                ClientId = _currentUser.ClientId,
                Active=true,

              };

              await _productCategoryRepository.CreateProductCategory(newCategory);

              allProductCategorys!.Add(newCategory);

            }
            else
            {
             //Category exists
              product.CategoryName = existingCategory.CategoryName;
            }
          }
          //Get alll option list
          UDTProductDetailResponse response = UDTproductDetailSimplified.ConvertoProductOrderDetail(dataList,client.DefaultProductStationId.GetValueOrDefault(), _mapper, productOptionLookups, allProductCategorys!, allstores!);
          serviceResult = new ServiceResultDTO(response.UDTOrderDetail!);

          if (!response.IsSuccessed)
          {
            serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed; 
            serviceResult.IsSuccess = false;
            var erMSg = response.Errors?.Select(x => new { x.IsSuccessed, x.Row, Msg = $"Please correct the following: " + string.Join(',', x.Msg) });
            var json = JsonConvert.SerializeObject(erMSg);
            serviceResult.IsSuccess = response.IsSuccessed;
            serviceResult.Errors?.Add("InvalidParameter", new[] { json });
          }

          //file delete 
          if (System.IO.File.Exists(filePath))
          {
            System.IO.File.Delete(filePath);
          }
          //add file delete functionality
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
   
  private UDTproductDetailSimplified AddProductData(IList<string> rowData, IList<string> columnNames)
  {
    var product = new UDTproductDetailSimplified()
    {
      Title = rowData[columnNames.IndexFor("Title")],
      Description = rowData[columnNames.IndexFor("Description")],
      Sku = rowData[columnNames.IndexFor("Sku")],
      CategoryName = rowData[columnNames.IndexFor("CategoryName")],
      StoreName = rowData[columnNames.IndexFor("StoreName")],
      QtyAvailable = int.Parse(rowData[columnNames.IndexFor("QtyAvailable")]),
      Weight = rowData[columnNames.IndexFor("Weight")].ToDecimal(),
      PurchasePrice = rowData[columnNames.IndexFor("Purchaseprice")].ToDecimal(),
      SalePrice = rowData[columnNames.IndexFor("Saleprice")].ToDecimal() 
    };
    return product;
  }
  #endregion
}




