using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShopifyFeature.Command.DeleteShopifyConfig;
public class DeleteShopifyConfigCommandHandler : RequestHandlerBase<DeleteShopifyConfigCommand, ServiceResultDTO>
{
  private readonly IShopifyPluginRepository _shopifyPluginRepository;

  public DeleteShopifyConfigCommandHandler(IShopifyPluginRepository shopifyPluginRepository, IServiceProvider serviceProvider, ILogger<DeleteShopifyConfigCommandHandler> logger) : base(serviceProvider, logger)
  { 
    _shopifyPluginRepository = shopifyPluginRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteShopifyConfigCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var clientId = new ClientId(new Guid(request.ClientId!));
      var employeeId = new EmployeeId(new Guid(request.ClientId!));

      var oClient = await _shopifyPluginRepository.GetClientById(clientId);
      if (oClient != null)
      {
        if (oClient.SecretKey == request.SecretKey)
        {
          var oShopifyConfig = await _shopifyPluginRepository.GetShopifyConfigById(request.SaleChannelConfigId, clientId!);
          if (oShopifyConfig == null)
          {
            throw new EntityNotFoundException("ShopifyConfig", request.SaleChannelConfigId);
          }
          //delete flag set
          oShopifyConfig.DeleteShopifyConfig(employeeId);
          var isDeleted = await _shopifyPluginRepository.DeleteShopifyConfig(oShopifyConfig);
          if (isDeleted == true)
          {
            serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = isDeleted, Message = NotificationConstants.Success });
          }
          else
          {
            serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = false, Message = NotificationConstants.DeleteError });
          }
          return serviceResult;
        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Invalid User secret key.");
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Invalid User Entity or Not found.");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
