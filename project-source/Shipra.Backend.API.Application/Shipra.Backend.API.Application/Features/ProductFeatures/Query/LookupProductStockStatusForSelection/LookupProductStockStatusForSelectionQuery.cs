using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.LookupProductStockStatusForSelection;
public class LookupProductStockStatusForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class LookupProductStockStatusForSelectionQueryHandler : RequestHandlerBase<LookupProductStockStatusForSelectionQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public LookupProductStockStatusForSelectionQueryHandler(IProductRepository productRepository,IServiceProvider serviceProvider, ILogger<LookupProductStockStatusForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(LookupProductStockStatusForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<LookupProductStockStatus> list = await _productRepository.GetLookupProductStockStatusForSelectionQuery();
      serviceResult = new ServiceResultDTO(list);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
