using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetShopifySessionTokenByShop;
public class GetShopifySessionTokenByShopQuery : IRequest<ServiceResultDTO>
{
  public string? ShopName { get; set; }
}
public class GetShopifySessionTokenByShopQueryHandler : RequestHandlerBase<GetShopifySessionTokenByShopQuery, ServiceResultDTO>
{
  private readonly IMasterDbRepository _masterDbRepository;

  public GetShopifySessionTokenByShopQueryHandler(IMasterDbRepository masterDbRepository ,IServiceProvider serviceProvider, ILogger<GetShopifySessionTokenByShopQueryHandler> logger) : base(serviceProvider, logger)
  {
    _masterDbRepository = masterDbRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetShopifySessionTokenByShopQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oShopiShop = await _masterDbRepository.GetShopifySessionsByShop(request.ShopName!);
      if (oShopiShop is null)
      {
        throw new EntityNotFoundException("ShopifySessions", request.ShopName!);
      } 
      serviceResult = new ServiceResultDTO(new {  token = oShopiShop.Token, shopName = request.ShopName });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetShopifySessionTokenByShopQueryValidator : AbstractValidator<GetShopifySessionTokenByShopQuery>
{
  public GetShopifySessionTokenByShopQueryValidator()
  {
    RuleFor(x => x.ShopName).NotEmpty().NotNull();
  }
}
