using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.DeleteSaleChannelConfig;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.DeleteShopifySaleChannelConfig;


public class DeleteShopifySaleChannelConfigCommandHandler : RequestHandlerBase<DeleteShopifySaleChannelConfigCommand, ServiceResultDTO>
{
  private readonly IShopifyPluginRepository _shopifyPluginRepository;

  public DeleteShopifySaleChannelConfigCommandHandler(IShopifyPluginRepository shopifyPluginRepository, IServiceProvider serviceProvider, ILogger<DeleteSaleChannelConfigCommandHandler> logger) : base(serviceProvider, logger)
  { 
    _shopifyPluginRepository = shopifyPluginRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteShopifySaleChannelConfigCommand request, CancellationToken cancellationToken)
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
          var oSaleChannelConfig = await _shopifyPluginRepository.GetSaleChannelConfigForUpdateById(request.SaleChannelConfigId, clientId!);
          if (oSaleChannelConfig == null)
          {
            throw new EntityNotFoundException("SaleChannelConfig ", request.SaleChannelConfigId);
          }
          //delete flag set
          oSaleChannelConfig.DeleteSaleChannelConfig(employeeId);
          bool isDeleted = await _shopifyPluginRepository.DeleteSaleChannelConfig(oSaleChannelConfig);
          if (isDeleted == true)
          {
            serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = isDeleted, Message = NotificationConstants.DeleteSuccess });
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
