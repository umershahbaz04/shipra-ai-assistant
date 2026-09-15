using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.UpdateShipperInvoiceStatus;
public class UpdateShipperInvoiceStatusCommand : IRequest<ServiceResultDTO>
{
  public int? ShipperInvoiceId { get; set; }
  public int? InvoiceStatusId { get; set; }
  public string? RefNo { get; set; }
}
public class UpdateShipperInvoiceStatusCommandHandler : RequestHandlerBase<UpdateShipperInvoiceStatusCommand, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;

  public UpdateShipperInvoiceStatusCommandHandler(IShipperInvoiceRepository shipperInvoiceRepository,IServiceProvider serviceProvider, ILogger<UpdateShipperInvoiceStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateShipperInvoiceStatusCommand request, CancellationToken cancellationToken)
  {
    var result = new ServiceResultDTO();

    try
    {
      //var status = (int)EnumInvoiceStatus.Draft; 
      var oInvoice = await _shipperInvoiceRepository.GetShipperInvoiceById(request.ShipperInvoiceId.GetValueOrDefault(),_currentUser.ClientId!.Value!);

      if (oInvoice is null)
      {
        throw new EntityNotFoundException("Error ", request.ShipperInvoiceId!);
      }
      
      oInvoice.UpdateStatus(request.InvoiceStatusId!.Value, request.RefNo, _currentUser.UserName!);
      await _shipperInvoiceRepository.UpdateShipperInvoice(oInvoice);
      result = new ServiceResultDTO(new BaseResponseDto { Data = oInvoice.ShipperInvoiceId, Message = "Update successfully"});
      return result;
    }
    catch (Exception ex)
    {
      result.CreateErrorResponse(ex);
      return result;
    }
  }
}
public class UpdateShipperInvoiceStatusCommandValidator : AbstractValidator<UpdateShipperInvoiceStatusCommand>
{
  public UpdateShipperInvoiceStatusCommandValidator()
  {
    RuleFor(x => x.ShipperInvoiceId).NotNull().NotEmpty();
    RuleFor(x => x.InvoiceStatusId).NotNull().NotEmpty();
    // ✅ RefNo required ONLY when status = Paid
    When(x => x.InvoiceStatusId == (int)EnumInvoiceStatus.Paid, () =>
    {
      RuleFor(x => x.RefNo)
          .NotEmpty()
          .WithMessage("RefNo is required when invoice status is Paid.");
    });
  } 
}
