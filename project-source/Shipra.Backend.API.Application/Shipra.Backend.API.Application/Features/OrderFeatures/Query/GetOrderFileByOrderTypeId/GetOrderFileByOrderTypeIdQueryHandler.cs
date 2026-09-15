using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetFullfilableOrderFile;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderFileByOrderTypeId;

public class GetOrderFileByOrderTypeIdQueryHandler : RequestHandlerBase<GetOrderFileByOrderTypeIdQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetOrderFileByOrderTypeIdQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetOrderFileByOrderTypeIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderFileByOrderTypeIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<OrderUplaodSampleFile> listData = await _orderRepository.GetOrderUplaodSampleFile();
      var targetObj = listData.FirstOrDefault(x => x.CountryId == request.CountryId && x.OrderTypeId == request.OrderTypeId);

      string? fileUrl = string.Empty;
      if (targetObj is not null)
      {
        fileUrl = targetObj.FilePath;
        serviceResult = new ServiceResultDTO(new { url = fileUrl });
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
