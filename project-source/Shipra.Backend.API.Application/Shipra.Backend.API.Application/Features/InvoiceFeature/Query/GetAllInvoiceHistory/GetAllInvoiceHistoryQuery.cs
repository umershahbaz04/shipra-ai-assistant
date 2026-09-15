using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.InvoiceUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.InvoiceFeature.Query.GetAllInvoiceHistory;
public class GetAllInvoiceHistoryQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllInvoiceHistoryQueryHandler : RequestHandlerBase<GetAllInvoiceHistoryQuery, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedStripeRepository _sharedStripeRepository;

  public GetAllInvoiceHistoryQueryHandler(IConfigRepository configRepository, ISharedStripeRepository sharedStripeRepository, IServiceProvider serviceProvider, ILogger<GetAllInvoiceHistoryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _sharedStripeRepository = sharedStripeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllInvoiceHistoryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Integration Value");
      }

      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      string? result = await _sharedStripeRepository.GetAllShipraInvoicesByClient(_currentUser.ClientIdStr!, mcconfig.Value!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortDir, filter.SortCol!);

      var deseralisedResponse = JsonConvert.DeserializeObject<AdminApiResponseModel<List<InvoiceHistoryDto>>>(result);

      if (deseralisedResponse is not null && deseralisedResponse.totalCount > 0)
      {
        serviceResult = new ServiceResultDTO(new{ list = deseralisedResponse.list! , TotalCount  = deseralisedResponse.totalCount});
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
