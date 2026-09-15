using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllRefundTypeLookupForSelection;
public class GetAllRefundTypeLookupForSelectionQuery : IRequest<ServiceResultDTO>
{
  public string? ClientId { get; set; }
}
public class GetAllRefundTypeLookupForSelectionQueryHandler : RequestHandlerBase<GetAllRefundTypeLookupForSelectionQuery, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public GetAllRefundTypeLookupForSelectionQueryHandler(IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<GetAllRefundTypeLookupForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllRefundTypeLookupForSelectionQuery request, CancellationToken cancellationToken)
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
      var list = await _returnRepository.GetAllRefundTypeLookupForSelection(new Core.ClientAggregate.ClientId(GuidHelper.GetGuidFromString(clientId)));
      var defaulItem = RefundTypeLookup.AddDefault();
      list!.Add(defaulItem);
      var data = list!.Select(x => new
      {
        x.RefundTypeId,
        x.RefundTypeName
      }).OrderBy(x => x.RefundTypeId).ToList();
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
