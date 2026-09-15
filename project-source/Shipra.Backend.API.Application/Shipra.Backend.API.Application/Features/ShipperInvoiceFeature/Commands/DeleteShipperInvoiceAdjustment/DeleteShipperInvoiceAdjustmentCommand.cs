using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.DeleteShipperInvoiceAdjustment;
public class DeleteShipperInvoiceAdjustmentCommand : IRequest<ServiceResultDTO>
{
  public int? ShipperInvoiceAdjustmentId { get; set; }
}
public class DeleteShipperInvoiceAdjustmentCommandHandler : RequestHandlerBase<DeleteShipperInvoiceAdjustmentCommand, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public DeleteShipperInvoiceAdjustmentCommandHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<DeleteShipperInvoiceAdjustmentCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteShipperInvoiceAdjustmentCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var mapData = await _shipperInvoiceRepository.GetShipperInvoiceAdjustmentById(request.ShipperInvoiceAdjustmentId.GetValueOrDefault(), _currentUser.ClientId!.Value);
      if (mapData is null)
      {
        throw new EntityNotFoundException("Error ", request.ShipperInvoiceAdjustmentId!);
      }
      await _shipperInvoiceRepository.DeleteShipperInvoiceAdjustment(mapData);
      serviceResult = new ServiceResultDTO(mapData);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
