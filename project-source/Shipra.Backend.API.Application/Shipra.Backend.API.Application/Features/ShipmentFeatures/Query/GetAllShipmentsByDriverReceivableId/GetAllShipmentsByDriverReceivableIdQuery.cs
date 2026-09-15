using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.ExportShipmentsByDriverReceivableId;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetAllShipmentsByDriverReceivableId;
public class GetAllShipmentsByDriverReceivableIdQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? DriverReceivableId { get; set; }
}
public class GetAllShipmentsByDriverReceivableIdQueryHandler : RequestHandlerBase<GetAllShipmentsByDriverReceivableIdQuery, ServiceResultDTO>
{
  private readonly IShipmentRepository _shipmentRepository;

  public GetAllShipmentsByDriverReceivableIdQueryHandler(IShipmentRepository shipmentRepository, IServiceProvider serviceProvider, ILogger<GetAllShipmentsByDriverReceivableIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _shipmentRepository = shipmentRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllShipmentsByDriverReceivableIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      var settingOpDashboard = await _shipmentRepository.GetAllShipmentsByDriverReceivableId(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,request.DriverReceivableId, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(settingOpDashboard);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetAllShipmentsByDriverReceivableIdQueryValidator : AbstractValidator<GetAllShipmentsByDriverReceivableIdQuery>
{
  public GetAllShipmentsByDriverReceivableIdQueryValidator()
  {
    RuleFor(x => x.DriverReceivableId).NotEmpty().NotNull();
  }
}

