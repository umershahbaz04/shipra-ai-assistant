using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductStockHistoryByStockId;
public class GetProductStockHistoryByStockIdQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public long ProductStockId { get; set; }
  public int ReasonId { get; set; } 
}
public class GetProductStockHistoryByStockIdQueryHandler : RequestHandlerBase<GetProductStockHistoryByStockIdQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public GetProductStockHistoryByStockIdQueryHandler(IProductRepository productRepository,IServiceProvider serviceProvider, ILogger<GetProductStockHistoryByStockIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetProductStockHistoryByStockIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      dynamic data = await _productRepository.GetProductStockHistoryByStockIdAsync(request.ProductStockId, request.ReasonId, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw; 
    }
  }
}
