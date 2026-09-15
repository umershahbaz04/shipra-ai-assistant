using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetMenuItemPermissions;

public class GetMenuItemPermissionsQuery : IRequest<ServiceResultDTO>
{
  public long ClientUserRoleId { get; set; }
}

public class GetMenuItemPermissionsQueryHandler : RequestHandlerBase<GetMenuItemPermissionsQuery, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public GetMenuItemPermissionsQueryHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<GetMenuItemPermissionsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetMenuItemPermissionsQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _permissionRepository.GetMenuItemPermissions(_currentUser.ClientId!, request.ClientUserRoleId);
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
