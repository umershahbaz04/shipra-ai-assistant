using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.LeadAggregate;
using System.Net;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Command;

public class UploadLeadsCommand : IRequest<ServiceResultDTO>
{
  public IFormFile? File { get; set; }
}

public class UploadLeadsCommandHandler : RequestHandlerBase<UploadLeadsCommand, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly ILeadRepository _leadRepository;
  private readonly IOrderRepository _orderRepository;

  public UploadLeadsCommandHandler(IWebHostEnvironment webHostEnvironment, ILeadRepository leadRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<UploadLeadsCommandHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _leadRepository = leadRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UploadLeadsCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      IList<LeadUploadDTO>? dataList = null;

      string? uniqueFileName = null;
      if (request.File?.ContentType != null)
      {
        string fileUploadsFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
        if (!Directory.Exists(fileUploadsFolder))
        {
          Directory.CreateDirectory(fileUploadsFolder);
        }
        uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File?.FileName;
        string filePath = System.IO.Path.Combine(fileUploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          request.File?.CopyTo(fileStream);
        }

        dataList = ExcelReader.GetDataToList(filePath, AddLeadData);

        if (dataList != null && dataList.Count > 0)
        {
          var response = new LeadUploadResponseDTO();
          var successList = new List<LeadUploadDTO>();

          // Extract all lead mobile numbers to check in bulk
          var leadMobiles = dataList
            .Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Length >= 8)
            .Select(x => x.PhoneNumber!)
            .Distinct()
            .ToList();

          var bulkDuplicates = await _orderRepository.CheckMobileNosDuplicateBulk(leadMobiles, _currentUser?.ClientIdStr ?? "");

          int rowIndex = 1;
          foreach (var item in dataList)
          {
            rowIndex++;
            var errors = new List<string>();
            if (string.IsNullOrEmpty(item.PhoneNumber))
            {
              errors.Add("PhoneNumber is required.");
            }
            if (string.IsNullOrEmpty(item.ProductName))
            {
              errors.Add("ProductName is required.");
            }
            if (!string.IsNullOrEmpty(item.PhoneNumber) && item.PhoneNumber.Length >= 8)
            {
              var suffix = item.PhoneNumber.Substring(item.PhoneNumber.Length - 8);
              var dupResult = bulkDuplicates.FirstOrDefault(d => d.IsDuplicate && d.OrderNo != null && 
                d.MobileNo != null && d.MobileNo.EndsWith(suffix));

              if (dupResult != null && dupResult.IsDuplicate)
              {
                errors.Add($"Duplicate Mobile! Match found in Order: {dupResult.OrderNo} ({dupResult.DaysAgo} days ago)");
              }
            }
            if (errors.Any())
            {
              response.IsSuccessed = false;
              response.Errors.Add(new LeadUploadErrorDTO { Row = rowIndex.ToString(), Msg = errors, Item = item });
              continue;
            }
            await Task.Delay(1);
            successList.Add(item);
          }

          serviceResult = new ServiceResultDTO(new { successList, errors = response.Errors });

          if (!response.IsSuccessed)
          {
            serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            serviceResult.IsSuccess = false;
            var erMSg = response.Errors?.Select(x => new { x.IsSuccessed, x.Row, Msg = $"Please correct the following: " + string.Join(',', x.Msg), x.Item });
            var json = JsonConvert.SerializeObject(erMSg);
            serviceResult.Errors?.Add("InvalidParameter", new[] { json });
          }
        }
        if (System.IO.File.Exists(filePath))
        {
          System.IO.File.Delete(filePath);
        }
      }
    }
    catch (Exception ex)
    {
      serviceResult.IsSuccess = false;
      serviceResult.Errors?.Add("Exception", new[] { ex.Message });
      serviceResult.StatusCode = (int)HttpStatusCode.InternalServerError;
    }

    return serviceResult;
  }

  private LeadUploadDTO AddLeadData(IList<string> rowData, IList<string> columnNames)
  {
    var leadDTO = new LeadUploadDTO();
    for (int i = 0; i < columnNames.Count; i++)
    {
      string colName = columnNames[i].Trim().ToLower().Replace(" ", "");
      string val = i < rowData.Count ? rowData[i]?.Trim() ?? "" : "";
      switch (colName)
      {
        case "phonenumber":
          leadDTO.PhoneNumber = val;
          break;
        case "productname":
          leadDTO.ProductName = val;
          break;
        case "googlelocationlink":
          leadDTO.GoogleLocationLink = val;
          break;
      }
    }
    return leadDTO;
  }
}

public class LeadUploadDTO
{
  public string? PhoneNumber { get; set; }
  public string? ProductName { get; set; }
  public string? GoogleLocationLink { get; set; }
  public int? CountryId { get; set; }
  public Guid? SalespersonId { get; set; }
}

public class LeadUploadResponseDTO
{
  public bool IsSuccessed { get; set; } = true;
  public List<LeadUploadErrorDTO> Errors { get; set; } = new List<LeadUploadErrorDTO>();
}

public class LeadUploadErrorDTO
{
  public bool IsSuccessed { get; set; } = false;
  public string Row { get; set; } = string.Empty;
  public List<string> Msg { get; set; } = new List<string>();
  public LeadUploadDTO? Item { get; set; }
}
