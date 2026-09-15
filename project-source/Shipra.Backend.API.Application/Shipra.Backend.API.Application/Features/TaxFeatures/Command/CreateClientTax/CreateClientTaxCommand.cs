using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.TaxFeatures.Command.CreateClientTax;
public class CreateClientTaxCommand : IRequest<ServiceResultDTO>
{
  public int? TaxId { get; set; }
  public decimal? Percentage { get; set; }
}
public class CreateClientTaxCommandHandler : RequestHandlerBase<CreateClientTaxCommand, ServiceResultDTO>
{
  private readonly ITaxRepository _taxRepository;

  public CreateClientTaxCommandHandler(ITaxRepository taxRepository, IServiceProvider serviceProvider, ILogger<CreateClientTaxCommandHandler> logger) : base(serviceProvider, logger)
  {
    _taxRepository = taxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientTaxCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var existOClientTax = await _taxRepository.GetAddedTaxById(request.TaxId.GetValueOrDefault(), _currentUser.ClientId!);

      if (existOClientTax is null)
      { 
        var oClientTax = ClientTax.CreateClientText(request.TaxId, request.Percentage, _currentUser.ClientId!, _currentUser.EmployeeId);
        oClientTax = await _taxRepository.CreateClientTax(oClientTax);
        if (oClientTax!.ClientTaxId > 0)
        {
          serviceResult = new ServiceResultDTO(new BaseResponseDto
          {
            Data = new { oClientTax.ClientTaxId, oClientTax.Percentage },
            Message = "Created Successfully"
          });
        }
      }
      else
      { 
        serviceResult.CreateError("AlreadyExist", new string[] { $"Tax already exist." });
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
