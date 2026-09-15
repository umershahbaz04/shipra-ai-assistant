using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllReturnStatusLookupForSelection;
public class GetAllReturnStatusLookupForSelection : IRequest<ServiceResultDTO>
{
  public string? ClientId { get; set; }
}
public class GetAllReturnStatusLookupForSelectionHandler : RequestHandlerBase<GetAllReturnStatusLookupForSelection, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public GetAllReturnStatusLookupForSelectionHandler(IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<GetAllReturnStatusLookupForSelectionHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllReturnStatusLookupForSelection request, CancellationToken cancellationToken)
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
      var list = await _returnRepository.GetAllReturnStatusLookupForSelection(new ClientId(new Guid(clientId)));
      var defaulItem = ReturnStatusLookup.AddDefault();
      list!.Add(defaulItem);
      var data = list!.Select(x => new
      {
        x.ReturnStatusLookupId,
        x.ReturnStatus
      }).OrderBy(x => x.ReturnStatusLookupId).ToList();
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
