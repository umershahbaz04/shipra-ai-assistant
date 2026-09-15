using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductStoresForSelection;
public class GetProductStocksForSelectionQuery : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public int StationId { get; set; }
  public string? Search { get; set; }
  public long? ProductVariantId { get; set; }
  public long? InventoryBalanceId { get; set; }
}
public class GetProductStoresForSelectionQueryHandler : RequestHandlerBase<GetProductStocksForSelectionQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IProductRepository _productRepository;
  public GetProductStoresForSelectionQueryHandler(IClientRepository clientRepository,IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetProductStoresForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetProductStocksForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (request.StoreId == 0)
      {
        request.StoreId = client!.DefaultStoreId.GetValueOrDefault();
      }
      var data = await _productRepository.GetProductStocksForSelection(_currentUser.ClientIdStr!, request.StoreId, request.StationId, request.Search, request.ProductVariantId, request.InventoryBalanceId);
      serviceResult = new ServiceResultDTO(data!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
