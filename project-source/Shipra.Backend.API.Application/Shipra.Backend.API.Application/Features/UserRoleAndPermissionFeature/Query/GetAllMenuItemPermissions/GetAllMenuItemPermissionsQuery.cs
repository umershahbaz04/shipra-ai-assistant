using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllMenuItemPermissions;

public class GetAllMenuItemPermissionsQuery : IRequest<ServiceResultDTO>
{
  public long ClientUserRoleId { get; set; }
}

public class GetAllMenuItemPermissionsQueryHandler : RequestHandlerBase<GetAllMenuItemPermissionsQuery, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public GetAllMenuItemPermissionsQueryHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<GetAllMenuItemPermissionsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllMenuItemPermissionsQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _permissionRepository.GetAllMenuItemPermissions(_currentUser.ClientId!, request.ClientUserRoleId);
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
