using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.AddUpdateClientRolePermissionGroup;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.UpdatePermissionAction;
public class UpdatePermissionActionCommand : IRequest<ServiceResultDTO>
{
  public string ControllerName { get; set; } = string.Empty;
  public List<string> Actions { get; set; } = new();
  public int PermissionGroupId { get; set; }
}
public class UpdatePermissionActionCommandCommandHandler : RequestHandlerBase<UpdatePermissionActionCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public UpdatePermissionActionCommandCommandHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<UpdatePermissionActionCommandCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(UpdatePermissionActionCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      foreach (var action in request.Actions)
      {
        var permissionAction = await _permissionRepository.GetPermissionActionByIdAsync(request.ControllerName, action);
        if (permissionAction is not null)
        {
          permissionAction.Update(request.PermissionGroupId); 

          await _permissionRepository.UpdatePermissionAction(permissionAction);
        } 
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

