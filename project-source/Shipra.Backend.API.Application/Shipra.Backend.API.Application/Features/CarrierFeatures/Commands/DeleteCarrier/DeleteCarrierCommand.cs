using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.DeleteCarrier;
 
public class DeleteCarrierCommand : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
}
public class DeleteCarrierCommandHandler : RequestHandlerBase<DeleteCarrierCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public DeleteCarrierCommandHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<DeleteCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteCarrierCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var carrier = await _carrierRepository.GetCarrierById(request.CarrierId);
      if (carrier == null)
      {
        throw new EntityNotFoundException("Carrier", request.CarrierId);
      }

      serviceResult.IsSuccess = await _carrierRepository.DeleteCarrier(carrier!);
      if (serviceResult.IsSuccess)
      {
        var result = new BaseResponseDto()
        {
          Message = NotificationConstants.Success
        };
        serviceResult = new ServiceResultDTO(result);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        return serviceResult;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.Error));
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
public class DeleteCarrierCommandValidator : AbstractValidator<DeleteCarrierCommand>
{
}
