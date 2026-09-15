using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNotePaymentInfo;
public class GetDeliveryNotePaymentInfoQueryHandler : RequestHandlerBase<GetDeliveryNotePaymentInfoQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public GetDeliveryNotePaymentInfoQueryHandler(IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<GetDeliveryNotePaymentInfoQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDeliveryNotePaymentInfoQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var paymentInfo = await _deliveryNoteRepository.GetDeliveryNotePaymentInfo(_currentUser.ClientIdStr!);
      if (paymentInfo is not null)
      {
        serviceResult = new ServiceResultDTO(paymentInfo);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      }
      else
      {
        serviceResult = new ServiceResultDTO("Delivery note payment detail not found");
        serviceResult.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest);
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
