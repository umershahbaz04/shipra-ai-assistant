using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllProducts;
public class GetAllProductsQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? StoreId { get; set; } 
  public bool? AddOptions { get; set; } 
  public int? ProductStationId { get; set; } 
}
public class GetAllProductsQueryHandler : RequestHandlerBase<GetAllProductsQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  public GetAllProductsQueryHandler(IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetAllProductsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllProductsQuery request, CancellationToken cancellationToken)
  {
    var serviceResultDTO = new ServiceResultDTO();

    try
    { 
      var filter = request.FilterModel!;
      var data = await _productRepository.GetAllProducts(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientId!.Value.ToString(),request.StoreId,request.AddOptions,request.ProductStationId);

      serviceResultDTO = new ServiceResultDTO(data);

      return serviceResultDTO;
    }
    catch (Exception ex)
    {

      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
