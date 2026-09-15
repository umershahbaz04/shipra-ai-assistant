using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.UpdatePermissionAction;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.UpdateControllerAndActionsRecord;
public class UpdateControllerAndActionsRecordCommand : IRequest<ServiceResultDTO>
{
  public List<ControllerInfo>? mvcControllers { get; set; } = new List<ControllerInfo>();
}
public class UpdateControllerAndActionsRecordCommandHandler : RequestHandlerBase<UpdateControllerAndActionsRecordCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public UpdateControllerAndActionsRecordCommandHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<UpdateControllerAndActionsRecordCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(UpdateControllerAndActionsRecordCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var newPermissions = new List<PermissionAction>();

      foreach (var controller in request.mvcControllers!)
      {
        foreach (var actionName in controller.Actions)
        {
          newPermissions.Add(new PermissionAction
          {
            ActionName = actionName,
            ControllerName = controller.ControllerName,
            PermissionGroupId = 1,
            Active = true
          });
        }
      }

      await _permissionRepository.CreatePermissionActions(newPermissions);

      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Data = "",
        Message = "Permission Update successfully"
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
