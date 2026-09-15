using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetFullfilableOrderFile;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetSampleExcelFileForStoreUpload;
public class GetSampleExcelFileForStoreUploadQuery : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
}
public class GetSampleExcelFileForStoreUploadQueryHandler : RequestHandlerBase<GetSampleExcelFileForStoreUploadQuery, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;

  public GetSampleExcelFileForStoreUploadQueryHandler(IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<GetSampleExcelFileForStoreUploadQuery> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetSampleExcelFileForStoreUploadQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<StoreUploadSampleFile> listData = await _storeRepository.GetSampleExcelFileForStoreUpload(request.CountryId);
      if (listData is not null && listData.Count > 0)
      {
        serviceResult = new ServiceResultDTO(listData);
        serviceResult.CreateSuccessResponse();
      }
      else
      {
        serviceResult.CreateError("Notfound", new string[] { "No file found." });
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
