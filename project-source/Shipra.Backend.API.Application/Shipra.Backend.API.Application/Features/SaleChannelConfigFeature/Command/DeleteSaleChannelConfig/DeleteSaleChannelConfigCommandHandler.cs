using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.DeleteSaleChannelConfig;

public class DeleteSaleChannelConfigCommandHandler : RequestHandlerBase<DeleteSaleChannelConfigCommand, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;

  public DeleteSaleChannelConfigCommandHandler(ISaleChannelConfigRepository SaleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<DeleteSaleChannelConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteSaleChannelConfigCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSaleChannelConfig = await _SaleChannelConfigRepository.GetSaleChannelConfigForUpdateById(request.SaleChannelConfigId, _currentUser.ClientId!);
      if (oSaleChannelConfig == null)
      {
        throw new EntityNotFoundException("SaleChannelConfig ", request.SaleChannelConfigId);
      }
      //delete flag set
      oSaleChannelConfig.DeleteSaleChannelConfig(_currentUser.EmployeeId);
      bool isDeleted = await _SaleChannelConfigRepository.DeleteSaleChannelConfig(oSaleChannelConfig);
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
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
