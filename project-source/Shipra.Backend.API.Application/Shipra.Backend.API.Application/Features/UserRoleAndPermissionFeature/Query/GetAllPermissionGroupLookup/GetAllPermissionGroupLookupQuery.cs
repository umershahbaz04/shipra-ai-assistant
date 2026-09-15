using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllPermissionActionForSelection;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllPermissionGroupLookup;
public class GetAllPermissionGroupLookupQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllPermissionGroupLookupQueryHandler : RequestHandlerBase<GetAllPermissionGroupLookupQuery, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public GetAllPermissionGroupLookupQueryHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<GetAllPermissionGroupLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPermissionGroupLookupQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var allPermissionGroups = await _permissionRepository.GetAllPermissionGroupLookup();

      var transformedData = allPermissionGroups.Select(p => new
      {
        p.PermissionGroupId,
        PermissionGroup = $"{p.GroupName!.Trim()}-{p.GroupParent!.Trim()}",
        p.Active
      }).ToList();

      serviceResult = new ServiceResultDTO(transformedData);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
