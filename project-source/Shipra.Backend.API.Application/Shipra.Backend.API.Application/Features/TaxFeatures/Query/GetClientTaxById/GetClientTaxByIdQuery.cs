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

namespace Shipra.Backend.API.Application.Features.TaxFeatures.Query.GetClientTaxById;
public class GetClientTaxByIdQuery : IRequest<ServiceResultDTO>
{
  public int? ClientTaxId { get; set; }
}
public class GetClientTaxByIdQueryHandler : RequestHandlerBase<GetClientTaxByIdQuery, ServiceResultDTO>
{
  private readonly ITaxRepository _taxRepository;

  public GetClientTaxByIdQueryHandler(ITaxRepository taxRepository, IServiceProvider serviceProvider, ILogger<GetClientTaxByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _taxRepository = taxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientTaxByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClientTax = await _taxRepository.GetClientTaxById(request.ClientTaxId.GetValueOrDefault(), _currentUser.ClientId!);

      if (oClientTax is null)
      {
        throw new EntityNotFoundException("ClientTax", request.ClientTaxId!);
      } 
      serviceResult = new ServiceResultDTO(new
      {
        oClientTax.ClientTaxId,
        oClientTax.TaxId,
        oClientTax.Percentage,
        oClientTax.CreatedOn, 
      });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetClientTaxByIdQueryValidator : AbstractValidator<GetClientTaxByIdQuery>
{
  public GetClientTaxByIdQueryValidator()
  {
    RuleFor(x => x.ClientTaxId).NotEmpty().NotNull().GreaterThan(0);
  }
}
