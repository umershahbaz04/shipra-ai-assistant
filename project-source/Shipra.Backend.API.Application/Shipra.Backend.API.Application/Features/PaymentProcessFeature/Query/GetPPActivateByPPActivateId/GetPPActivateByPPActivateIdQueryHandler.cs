using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetPPActivateByPPActivateId;
public class GetPPActivateByPPActivateIdQueryHandler : RequestHandlerBase<GetPPActivateByPPActivateIdQuery, ServiceResultDTO>
{
  private readonly IPaymentProcessRepository _paymentProcessRepository;

  public GetPPActivateByPPActivateIdQueryHandler(IPaymentProcessRepository paymentProcessRepository, IServiceProvider serviceProvider, ILogger<GetPPActivateByPPActivateIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _paymentProcessRepository = paymentProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPPActivateByPPActivateIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _paymentProcessRepository.GetPPActivateByPPActivateId(request.PPactivateId, _currentUser.ClientId!);
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
