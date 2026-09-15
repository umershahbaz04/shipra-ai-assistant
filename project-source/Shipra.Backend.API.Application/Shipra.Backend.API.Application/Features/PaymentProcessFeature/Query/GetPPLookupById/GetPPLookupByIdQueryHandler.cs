using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetPPLookupById;
public class GetPPLookupByIdQueryHandler : RequestHandlerBase<GetPPLookupByIdQuery, ServiceResultDTO>
{
  private readonly IPaymentProcessRepository _paymentProcessRepository;

  public GetPPLookupByIdQueryHandler(IPaymentProcessRepository paymentProcessRepository, IServiceProvider serviceProvider, ILogger<GetPPLookupByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _paymentProcessRepository = paymentProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPPLookupByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var pplookup = await _paymentProcessRepository.GetPPLookupById(request.PPLookupId);

      if (pplookup is null)
      {
        throw new EntityNotFoundException("Pplookup ", request.PPLookupId);
      }
      if (string.IsNullOrEmpty(pplookup!.InputRequiredConfig!))
      {
        throw new EntityNotFoundException("PaymentProcessConfig ", pplookup!.InputRequiredConfig!);
      }

      serviceResult = new ServiceResultDTO(pplookup!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

