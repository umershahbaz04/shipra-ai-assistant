using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nancy.Json;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllProducts;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetMarketPlaceProducts;
public class GetMarketPlaceProductsQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int marketPlaceLookupId { get; set; }
}
public class GetMarketPlaceProductsQueryHandler : RequestHandlerBase<GetMarketPlaceProductsQuery, ServiceResultDTO>
{
  private readonly ISharedStripeRepository _sharedStripeRepository;
  private readonly IConfigRepository _configRepository;
  public GetMarketPlaceProductsQueryHandler(ISharedStripeRepository sharedStripeRepository,IConfigRepository configRepository, IServiceProvider serviceProvider, ILogger<GetMarketPlaceProductsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _sharedStripeRepository = sharedStripeRepository;
    _configRepository = configRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetMarketPlaceProductsQuery request, CancellationToken cancellationToken)
  {
    var serviceResultDTO = new ServiceResultDTO();

    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Integration Value");
      }
      var filter = request.FilterModel!;
      var data = await _sharedStripeRepository.GetAllMarketPlaceProducts(_currentUser.ClientId!.Value.ToString(), request.marketPlaceLookupId, true,mcconfig.Value!, filter.Start, filter.Length, filter.Search!, filter.SortDir!,filter.SortCol);
      var obj = JsonConvert.DeserializeObject<object>(data) ?? new { };
      serviceResultDTO = new ServiceResultDTO(obj);

      return serviceResultDTO;
    }
    catch (Exception ex)
    {

      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
