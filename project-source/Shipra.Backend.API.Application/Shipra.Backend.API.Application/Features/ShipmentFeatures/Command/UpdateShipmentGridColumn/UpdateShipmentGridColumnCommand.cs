using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ShipmentUseCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.UpdateShipmentGridColumn;
public class UpdateShipmentGridColumnCommand : IRequest<ServiceResultDTO>
{
  public List<UpdateShipmentGridColumnRequestModel>? list { get; set; }
}
public class UpdateShipmentGridColumnCommandHandler : RequestHandlerBase<UpdateShipmentGridColumnCommand, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;

  public UpdateShipmentGridColumnCommandHandler(IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<UpdateShipmentGridColumnCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateShipmentGridColumnCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      foreach (var item in request.list!)
      {
        var oShipmentGridColumn = await _shipmentRepository.GetShipmentGridColumnById(item.shipmentGridColumnId,_currentUser.ClientId!);
        if (oShipmentGridColumn is not null)
        {
          oShipmentGridColumn.UpdateColumnName(item.columnName?.Trim().ToUpper(),_currentUser.EmployeeId!);
          await _shipmentRepository.UpdateShipmentGridColumn(oShipmentGridColumn);

          var oShipmentGridClientSetting = await _shipmentRepository.GetShipmentGridClientSettingByShipmentGridColumnId(item.shipmentGridColumnId, _currentUser.ClientId!);
          if (oShipmentGridClientSetting is not null)
          {
            oShipmentGridClientSetting.UpdateDashboardStatusValue(item.dashboardStatusValue!, _currentUser.EmployeeId);
            await _shipmentRepository.UpdateShipmentGridClientSetting(oShipmentGridClientSetting);
          }
        }
        serviceResult.IsSuccess = true;
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
public class UpdateShipmentGridColumnCommandValidator : AbstractValidator<UpdateShipmentGridColumnCommand>
{
  public UpdateShipmentGridColumnCommandValidator()
  {
    RuleFor(x => x.list).Must(x => x != null).WithMessage("Items list must contain at least one item.");
    RuleForEach(x => x.list).SetValidator(x => new CreateShipmentGridColumnRequestModelValidator());

  }
}
public class CreateShipmentGridColumnRequestModelValidator : AbstractValidator<UpdateShipmentGridColumnRequestModel>
{
  public CreateShipmentGridColumnRequestModelValidator()
  {
    RuleFor(v => v.shipmentGridColumnId).NotNull().GreaterThan(0);
    RuleFor(v => v.dashboardStatusValue).NotNull().NotEmpty();
    RuleFor(v => v.columnName).NotNull().NotEmpty(); 
  }
}
