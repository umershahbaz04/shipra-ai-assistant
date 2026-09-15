using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetAllPPActivate;
public class GetAllPPActivateQueryHandler : RequestHandlerBase<GetAllPPActivateQuery, ServiceResultDTO>
{
  private readonly IPaymentProcessRepository _paymentProcessRepository;

  public GetAllPPActivateQueryHandler(IPaymentProcessRepository paymentProcessRepository, IServiceProvider serviceProvider, ILogger<GetAllPPActivateQueryHandler> logger) : base(serviceProvider, logger)
  {
    _paymentProcessRepository = paymentProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPPActivateQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var data = await _paymentProcessRepository.GetAllPPActivate(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientIdStr!);
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
