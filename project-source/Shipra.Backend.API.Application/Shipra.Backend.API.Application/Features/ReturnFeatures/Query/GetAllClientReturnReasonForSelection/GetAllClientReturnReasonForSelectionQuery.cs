using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllClientReturnReasonForSelection;
public class GetAllClientReturnReasonForSelectionQuery : IRequest<ServiceResultDTO>
{
  public string? ClientId { get; set; }
}
public class GetAllClientReturnReasonForSelectionQueryHandler : RequestHandlerBase<GetAllClientReturnReasonForSelectionQuery, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public GetAllClientReturnReasonForSelectionQueryHandler(IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<GetAllClientReturnReasonForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientReturnReasonForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      string? clientId = !string.IsNullOrEmpty(request.ClientId) ? request.ClientId : _currentUser.ClientIdStr;
      if (string.IsNullOrEmpty(clientId))
      {
        serviceResult.CreateError("ClientIdNotFound", new string[] { "Client is required" });
        return serviceResult;
      }
      if (!GuidHelper.Validator(clientId))
      {
        serviceResult.CreateError("Invalid", new string[] { GuidHelper.GuidMessage });
        return serviceResult;
      }
      var list = await _returnRepository.GetAllClientReturnReasonForSelection(new Core.ClientAggregate.ClientId(GuidHelper.GetGuidFromString(clientId)));
      var defaulItem = ClientReturnReason.AddDefault();
      list!.Add(defaulItem);
      var data = list!.Select(x => new
      {
        x.ClientReturnReasonId,
        x.Reason
      }).OrderBy(x => x.ClientReturnReasonId).ToList();
      serviceResult = new ServiceResultDTO(data);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
