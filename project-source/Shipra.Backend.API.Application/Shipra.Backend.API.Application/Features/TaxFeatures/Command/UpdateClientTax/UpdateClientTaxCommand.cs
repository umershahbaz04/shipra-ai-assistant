using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.TaxFeatures.Command.UpdateClientTax;
public class UpdateClientTaxCommand : IRequest<ServiceResultDTO>
{
  public int ClientTaxId { get; set; }
  public int? TaxId { get; set; }
  public decimal? Percentage { get; set; } 
}
public class UpdateClientTaxCommandHandler : RequestHandlerBase<UpdateClientTaxCommand, ServiceResultDTO>
{
  private readonly ITaxRepository _taxRepository;

  public UpdateClientTaxCommandHandler(ITaxRepository taxRepository, IServiceProvider serviceProvider, ILogger<UpdateClientTaxCommandHandler> logger) : base(serviceProvider, logger)
  {
    _taxRepository = taxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientTaxCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClientTax = await _taxRepository.GetClientTaxById(request.ClientTaxId, _currentUser.ClientId!);

      if (oClientTax is null)
      {
        throw new EntityNotFoundException("ClientTax", request.ClientTaxId!);
      }

      oClientTax!.UpdateClientText(request.TaxId,request.Percentage, _currentUser.EmployeeId);
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
public class UpdateClientTaxCommandValidator : AbstractValidator<UpdateClientTaxCommand>
{
  public UpdateClientTaxCommandValidator()
  {
    RuleFor(x => x.ClientTaxId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.TaxId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.Percentage).NotEmpty().NotNull().GreaterThan(0);
  }
}

