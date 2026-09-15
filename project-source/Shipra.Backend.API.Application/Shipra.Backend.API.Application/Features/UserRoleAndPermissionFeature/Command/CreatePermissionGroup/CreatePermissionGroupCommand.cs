using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.CreatePermissionGroup;
public class CreatePermissionGroupCommand : IRequest<ServiceResultDTO>
{
  public string? GroupName { get; set; }
  public string? GroupParent { get; set; }
}
public class CreatePermissionGroupCommandHandler : RequestHandlerBase<CreatePermissionGroupCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public CreatePermissionGroupCommandHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<CreatePermissionGroupCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(CreatePermissionGroupCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      if (string.IsNullOrWhiteSpace(request.GroupName))
        throw new Exception("GroupName is required");

      var groupNames = request.GroupName
          .Split(',', StringSplitOptions.RemoveEmptyEntries)
          .Select(x => x.Trim())
          .ToList();

      foreach (var name in groupNames)
      {
        PermissionGroupLookup permissionGroupLookup = PermissionGroupLookup.Create(name, request.GroupParent!);
        await _permissionRepository.CreatePermissionGroupAsync(permissionGroupLookup);
      }
      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Data = "",
        Message = "Permission saved successfully"
      });
      return serviceResult;
    
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
