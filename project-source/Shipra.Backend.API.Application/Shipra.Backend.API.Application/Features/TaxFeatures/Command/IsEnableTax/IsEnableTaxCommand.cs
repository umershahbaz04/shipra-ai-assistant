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
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.TaxFeatures.Command.IsEnableTax;
public class IsEnableTaxCommand : IRequest<ServiceResultDTO>
{
  public int? ClientTaxId { get; set; }
  public bool? IsActive { get; set; }
}
public class IsEnableTaxCommandHandler : RequestHandlerBase<IsEnableTaxCommand, ServiceResultDTO>
{
  private readonly ITaxRepository _taxRepository;

  public IsEnableTaxCommandHandler(ITaxRepository taxRepository, IServiceProvider serviceProvider, ILogger<IsEnableTaxCommandHandler> logger) : base(serviceProvider, logger)
  {
    _taxRepository = taxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(IsEnableTaxCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClientTax = await _taxRepository.GetClientTaxById(request.ClientTaxId.GetValueOrDefault(), _currentUser.ClientId!);

      if (oClientTax is null)
      {
        throw new EntityNotFoundException("ClientTax", request.ClientTaxId!); 
      }

      oClientTax!.EnableDisableTax(request.IsActive, _currentUser.EmployeeId);
      var isAdded = await _taxRepository.UpdateClientTax(oClientTax);
      if (isAdded)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = request.ClientTaxId,
          Message = "Updated Successfully"
        });
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
public class IsEnableTaxCommandValidator : AbstractValidator<IsEnableTaxCommand>
{
  public IsEnableTaxCommandValidator()
  {
    RuleFor(x => x.ClientTaxId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.IsActive).NotEmpty().NotNull();
  }
}
