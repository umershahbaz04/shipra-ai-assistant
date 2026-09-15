using System.Dynamic;
using FluentValidation;
using iText.Layout;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GenerateManifest;
public class GenerateManifestQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}
public class GenerateManifestQueryHandler : RequestHandlerBase<GenerateManifestQuery, ServiceResultDTO>
{ 
  private readonly ISharedBarcodeRepository _sharedBarcodeRepository;
  private readonly IConfigRepository _configRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IOrderRepository _orderRepository;

  public GenerateManifestQueryHandler(ISharedBarcodeRepository sharedBarcodeRepository, IConfigRepository configRepository, IWebHostEnvironment webHostEnvironment, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GenerateManifestQueryHandler> logger) : base(serviceProvider, logger)
  { 
    _sharedBarcodeRepository = sharedBarcodeRepository;
    _configRepository = configRepository;
    _webHostEnvironment = webHostEnvironment;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GenerateManifestQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      if (string.IsNullOrEmpty(_currentUser.ClientIdStr))
      {
        serviceResult.CreateError("ClientIdNotFound", new string[] { "Client is required" });
        return serviceResult;
      }
      #region awb 
      var result = await _orderRepository.GetOrderInfoByOrderNo(request.OrderNos!, _currentUser.ClientIdStr!);
      if (Enumerable.Count(result) > 0)
      { 
        var convertedResult = ConvertOrderFromDynamic(result);

        var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.ShipraServiceKey, _currentUser.EnvironmentTypeId);
        if (mcconfig is null)
        {
          throw new EntityNotFoundException("Mcconfig", "Service Value");
        }

        Stream? pdfData = await GetAwbDataFromService(convertedResult, mcconfig.Value!);
        byte[] bytes = PDFDocumentGenerator.ConvertStreamToByteArray(pdfData);
        serviceResult = new ServiceResultDTO(bytes);
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
  private async Task<Stream> GetAwbDataFromService(List<OrderDetailsModel> result, string domainUrl)
  {
    dynamic manifestDataModel = new ExpandoObject();
    manifestDataModel.from = "";
    manifestDataModel.to = "";
    manifestDataModel.driver = "";
    manifestDataModel.manifestCode = GetNextShipperManifestCode();

    var orderDataList = new List<dynamic>();

    foreach (var data in result)
    {
      // Create a new dynamic object for each order
      dynamic orderData = new ExpandoObject();
      orderData.OrderNo = data.OrderNo;
      orderData.CarrierTrackingNo = data.CarrierTrackingNo;
      orderData.Description = data.Description;
      orderData.RefNo = data.RefNo; 
      orderData.Amount = data.Amount;
      orderData.ItemValue = data.ItemValue;

      orderDataList.Add(orderData);
    }

    manifestDataModel.orders = orderDataList;

    var jsonData = JsonConvert.SerializeObject(manifestDataModel, Formatting.Indented);
    var stream = await _sharedBarcodeRepository.GetManifest(jsonData, domainUrl);

    return stream;
  }
  public string GetNextShipperManifestCode()
  {
    // Get the current date and time in yyyyMMddHHmmss format
    string currentDateTimePart = "M" + DateTime.Now.ToString("yyyyMMddHHmm");

    // The manifest code is now just the current date/time part followed by '1'
    string shipperManifestCode = $"{currentDateTimePart}";

    return shipperManifestCode;
  }

  public List<OrderDetailsModel> ConvertOrderFromDynamic(dynamic result)
  {
    string serialized = JsonConvert.SerializeObject(result);
    var typedModel = JsonConvert.DeserializeObject<List<OrderDetailsModel>>(serialized);

    if (typedModel != null)
    {
      foreach (var x in typedModel)
      {
        x.ItemValue = x.Amount; // Assign the value of Amount to ItemValue for each order
        if (x.PaymentMethodId == (int)EnumPaymentMethod.PP)
        {
          x.Amount = 0;
        }
      }
    }

    return typedModel ?? new List<OrderDetailsModel>();
  }

}
public class GenerateManifestQueryValidator : AbstractValidator<GenerateManifestQuery>
{
  public GenerateManifestQueryValidator()
  {
    RuleFor(x => x.OrderNos).NotEmpty().NotNull();
  } 
}
