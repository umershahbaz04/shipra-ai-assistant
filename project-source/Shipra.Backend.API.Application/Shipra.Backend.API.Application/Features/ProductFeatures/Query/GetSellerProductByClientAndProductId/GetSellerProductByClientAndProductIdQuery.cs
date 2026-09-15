using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
using Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetProductById;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetSellerProductByClientAndProductId;
public class GetSellerProductByClientAndProductIdQuery : IRequest<ServiceResultDTOWithTypeModel<ProductResponseModel>>
{
  public string? ProductId { get; set; }
}
public class GetSellerProductByClientAndProductIdQueryHandler : RequestHandlerBase<GetSellerProductByClientAndProductIdQuery, ServiceResultDTOWithTypeModel<ProductResponseModel>>
{
  private readonly IMediator _mediator;

  public GetSellerProductByClientAndProductIdQueryHandler(IMediator mediator,IServiceProvider serviceProvider, ILogger<GetSellerProductByClientAndProductIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ProductResponseModel>> HandleRequest(GetSellerProductByClientAndProductIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<ProductResponseModel>();
    try
    {
      var questRequest = new GetProductByIdQuery() { ProductId = request.ProductId };

      serviceResult = await _mediator.Send(questRequest);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
