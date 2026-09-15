using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductStationUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Query.GetProductStationById;

public class GetProductStationByIdQueryHandler : RequestHandlerBase<GetProductStationByIdQuery, ServiceResultDTOWithTypeModel<ProductStationResposeModel>>
{
  private readonly IProductStationRepository _productStationRepository;

  public GetProductStationByIdQueryHandler(IProductStationRepository productStationRepository,IServiceProvider serviceProvider, ILogger<GetProductStationByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productStationRepository = productStationRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ProductStationResposeModel>> HandleRequest(GetProductStationByIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<ProductStationResposeModel>();
    try
    {
      var target = await _productStationRepository.GetProductStationById(request.ProductStationId); 
      if (target == null)
      {
        throw new EntityNotFoundException("Product Station", request.ProductStationId);
      }

      var model = _mapper.Map<ProductStationResposeModel>(target);

      serviceResult = new ServiceResultDTOWithTypeModel<ProductStationResposeModel>(model);
      serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
     
  }
}
