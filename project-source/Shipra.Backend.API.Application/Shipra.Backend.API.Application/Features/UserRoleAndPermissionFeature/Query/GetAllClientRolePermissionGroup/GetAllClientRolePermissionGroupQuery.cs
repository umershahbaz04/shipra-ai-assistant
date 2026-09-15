using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientRolePermissionGroup;
public class GetAllClientRolePermissionGroupQuery : IRequest<ServiceResultDTO>
{
  public int clientUserRoleId { get; set; }
}
public class GetAllClientRolePermissionGroupQueryHandler : RequestHandlerBase<GetAllClientRolePermissionGroupQuery, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public GetAllClientRolePermissionGroupQueryHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<GetAllClientRolePermissionGroupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientRolePermissionGroupQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    { 
      var allPermissionActions = await _permissionRepository.GetAllClientRolePermissionGroup(_currentUser.ClientId!,request.clientUserRoleId); 
      serviceResult = new ServiceResultDTO(allPermissionActions);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
