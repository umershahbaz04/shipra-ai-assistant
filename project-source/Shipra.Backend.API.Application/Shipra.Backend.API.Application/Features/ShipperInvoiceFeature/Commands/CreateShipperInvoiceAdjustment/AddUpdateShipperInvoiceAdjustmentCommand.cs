using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.CreateShipperInvoiceAdjustment;
public class AddUpdateShipperInvoiceAdjustmentCommand : IRequest<ServiceResultDTO>
{
  public int? ShipperInvoiceAdjustmentId { get; set; }
  public int? ShipperInvoiceId { get; set; }
  public int? TransactionTypeId { get; set; }
  public decimal? Amount { get; set; }
  public string? Comment { get; set; }
  public int SaleChannelConfigId { get; set; }
}
public class AddUpdateShipperInvoiceAdjustmentCommandHandler : RequestHandlerBase<AddUpdateShipperInvoiceAdjustmentCommand, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public AddUpdateShipperInvoiceAdjustmentCommandHandler(IShipperInvoiceRepository shipperInvoiceRepository, IServiceProvider serviceProvider, ILogger<AddUpdateShipperInvoiceAdjustmentCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AddUpdateShipperInvoiceAdjustmentCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      ShipperInvoiceAdjustment? objShipperInvoiceAdjustment;
      if (request.ShipperInvoiceAdjustmentId.GetValueOrDefault() > 0)
      {

        objShipperInvoiceAdjustment = await _shipperInvoiceRepository.GetShipperInvoiceAdjustmentById(request.ShipperInvoiceAdjustmentId.GetValueOrDefault(), _currentUser.ClientId!.Value);
        if (objShipperInvoiceAdjustment == null)
        {
          throw new EntityNotFoundException("ShipperInvoiceAdjustmentNotFound", request.ShipperInvoiceAdjustmentId!);
        }
        objShipperInvoiceAdjustment.Update(request.Amount!.Value, request.Comment, request.TransactionTypeId, "EmployeeName"!);
        await _shipperInvoiceRepository.UpdateShipperInvoiceAdjustment(objShipperInvoiceAdjustment);
      }
      else
      {
        objShipperInvoiceAdjustment = ShipperInvoiceAdjustment.Create(request.TransactionTypeId, request.Amount, request.Comment, _currentUser.ClientId!.Value, request.SaleChannelConfigId, "EmployeeName"!);
        await _shipperInvoiceRepository.CreateShipperInvoiceAdjustment(objShipperInvoiceAdjustment);
      }
      if (objShipperInvoiceAdjustment.ShipperInvoiceAdjustmentId != 0)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto()
        {
          Data = objShipperInvoiceAdjustment.ShipperInvoiceAdjustmentId,
          Message = NotificationConstants.Success,
        });
        return serviceResult;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.InvalidResponse));
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
