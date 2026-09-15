using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.Interfaces;
using System.Dynamic;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetCompletedDeliveryNoteExpenses;
public class GetCompletedDeliveryNoteExpensesQueryHandler : RequestHandlerBase<GetCompletedDeliveryNoteExpensesQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  public GetCompletedDeliveryNoteExpensesQueryHandler(IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<GetCompletedDeliveryNoteExpensesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCompletedDeliveryNoteExpensesQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _deliveryNoteRepository.GetCompletedDeliveryNoteExpenses(request.DeliveryNoteId!);

      serviceResult = new ServiceResultDTO(result);
      serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
