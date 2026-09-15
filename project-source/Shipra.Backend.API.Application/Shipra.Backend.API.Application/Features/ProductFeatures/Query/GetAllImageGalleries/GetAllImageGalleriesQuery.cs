using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
using Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateImageGallery;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllImageGalleries;
public class GetAllImageGalleriesQuery : IRequest<ServiceResultDTO>
{ 
}
public class GetAllImageGalleriesQueryHandler : RequestHandlerBase<GetAllImageGalleriesQuery, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;

  public GetAllImageGalleriesQueryHandler(IProductRepository productRepository,IServiceProvider serviceProvider, ILogger<GetAllImageGalleriesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllImageGalleriesQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var dataList =  await _productRepository.GetAllImageGalleries(_currentUser.ClientId!);
      var dataListMap = _mapper.Map<List<ImageGalleryResponseDto>>(dataList);

      serviceResult = new ServiceResultDTO(dataListMap!);  
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
