using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.StripeGetAllProducts;
public class StripeGetAllProductsQuery : IRequest<ServiceResultDTO>
{
}

public class StripeGetAllProductsQueryHandler : RequestHandlerBase<StripeGetAllProductsQuery, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedStripeRepository _sharedStripeRepository;

  public StripeGetAllProductsQueryHandler(IConfigRepository configRepository, ISharedStripeRepository sharedStripeRepository, IServiceProvider serviceProvider, ILogger<StripeGetAllProductsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _sharedStripeRepository = sharedStripeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(StripeGetAllProductsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      var result = await _sharedStripeRepository.GetShipraProductRecordsForAppDisplayByTenant(_currentUser.ClientIdStr!, mcconfig.Value!);
      if (!string.IsNullOrEmpty(result))
      {
        var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<List<StripeProductResponseModel>>>(result);
        if (deseralisedResponse!.isSuccess)
        {
          serviceResult = new ServiceResultDTO(deseralisedResponse.result!);
          serviceResult.Errors = deseralisedResponse.errors;
          serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
        }
        else
        {
          serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
        }
      }
      else
      {
        serviceResult.Errors!["StripeGetAllProductsQuery"] = new string[] { "Product List is Failed: " + _currentUser.ClientId };
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
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
