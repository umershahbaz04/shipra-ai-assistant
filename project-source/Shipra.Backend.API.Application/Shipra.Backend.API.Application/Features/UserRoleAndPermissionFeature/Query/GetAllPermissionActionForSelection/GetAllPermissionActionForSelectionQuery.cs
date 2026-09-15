using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientUserRole;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllPermissionActionForSelection;
public class GetAllPermissionActionForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllPermissionActionForSelectionQueryHandler : RequestHandlerBase<GetAllPermissionActionForSelectionQuery, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public GetAllPermissionActionForSelectionQueryHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<GetAllPermissionActionForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPermissionActionForSelectionQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var allUserRoles = await _permissionRepository.GetAllPermissionActionForSelection();
      serviceResult = new ServiceResultDTO(allUserRoles);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
