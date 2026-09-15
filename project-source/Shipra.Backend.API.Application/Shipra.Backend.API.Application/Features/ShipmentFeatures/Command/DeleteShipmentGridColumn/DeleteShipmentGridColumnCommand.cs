using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.DeleteShipmentGridColumn;
public class DeleteShipmentGridColumnCommand : IRequest<ServiceResultDTO>
{
  public int ShipmentGridColumnId { get; set; }
}
public class DeleteShipmentGridColumnCommandHandler : RequestHandlerBase<DeleteShipmentGridColumnCommand, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;

  public DeleteShipmentGridColumnCommandHandler(IShipmentRepository shipmentRepository,IServiceProvider serviceProvider, ILogger<DeleteShipmentGridColumnCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteShipmentGridColumnCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      //check name already exist 
      ShipmentGridColumn oExisShipmentGridColumn = await _shipmentRepository.GetShipmentGridColumnById(request.ShipmentGridColumnId, _currentUser.ClientId!);
      if (oExisShipmentGridColumn is null)
      {
        throw new EntityNotFoundException("ShipmentGridColumn ", request.ShipmentGridColumnId!); 
      }
      if (!oExisShipmentGridColumn.IsDefaultStatusTab.GetValueOrDefault())
      {
        #region grid client setting
        var oShipmentGridClientSetting = await _shipmentRepository.GetShipmentGridClientSettingByShipmentGridColumnId(oExisShipmentGridColumn.ShipmentGridColumnId!, _currentUser.ClientId!);
        if (oShipmentGridClientSetting is not null)
        {
          await _shipmentRepository.DeleteShipmentGridClientSetting(oShipmentGridClientSetting);
        }
        #endregion
        //delete here
        bool isDelete = await _shipmentRepository.DeleteShipmentGridColumn(oExisShipmentGridColumn);
        serviceResult.IsSuccess = isDelete;
        if (isDelete)
        {
          serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = request.ShipmentGridColumnId, Message = "Shipment Grid Tab Deleted successfully" });
        }
      }
      else
      {
        serviceResult.CreateError("DefaultTab", new string[] { $"You can't delete default tab." }); 
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
public class DeleteShipmentGridColumnCommandValidator : AbstractValidator<DeleteShipmentGridColumnCommand>
{
  public DeleteShipmentGridColumnCommandValidator()
  {
    RuleFor(x => x.ShipmentGridColumnId).NotNull().GreaterThan(0);
  }
}
