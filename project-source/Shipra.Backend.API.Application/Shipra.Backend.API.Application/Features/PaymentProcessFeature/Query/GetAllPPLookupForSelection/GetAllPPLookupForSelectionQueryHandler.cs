using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetAllPPLookup;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetAllPPLookupForSelection;
public class GetAllPPLookupForSelectionQueryHandler : RequestHandlerBase<GetAllPPLookupForSelectionQuery, ServiceResultDTO>
{
  private readonly IPaymentProcessRepository _paymentProcessRepository;

  public GetAllPPLookupForSelectionQueryHandler(IPaymentProcessRepository paymentProcessRepository, IServiceProvider serviceProvider, ILogger<GetAllPPLookupForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _paymentProcessRepository = paymentProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPPLookupForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _paymentProcessRepository.GetAllPPLookupForSelection();
      serviceResult = new ServiceResultDTO(data!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

