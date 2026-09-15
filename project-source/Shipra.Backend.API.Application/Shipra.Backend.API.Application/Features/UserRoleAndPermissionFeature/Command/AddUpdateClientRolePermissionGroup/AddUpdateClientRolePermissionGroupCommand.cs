using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.PermissionUseCase;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.AddUpdateClientRolePermissionGroup;
public class AddUpdateClientRolePermissionGroupCommand : IRequest<ServiceResultDTO>
{
  public List<AddUpdateClientRolePermissionGroupRequestModel>? listItems { get; set; }
  public int ClientUserRoleId { get; set; }
}
public class AddUpdateClientRolePermissionGroupCommandHandler : RequestHandlerBase<AddUpdateClientRolePermissionGroupCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public AddUpdateClientRolePermissionGroupCommandHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<AddUpdateClientRolePermissionGroupCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AddUpdateClientRolePermissionGroupCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      if (request.listItems!.Count > 0)
      {
        foreach (var item in request.listItems)
        {
          if (item.ClientRolePgid == 0 && item.HavePermission.GetValueOrDefault(false))
          {
            ClientRolePermissionGroup oClientRolePermissionGroup = ClientRolePermissionGroup.Create(request.ClientUserRoleId, item.RolePermissionGroupId, _currentUser.ClientId!);
            await _permissionRepository.CreateClientRolePermissionGroup(oClientRolePermissionGroup);
          }
          else
          {
            //check if no permission removed but entry exist then delete it
            if (!item.HavePermission.GetValueOrDefault(false))
            {
              ClientRolePermissionGroup? oClientRolePermissionGroup = await _permissionRepository.GetClientRolePermissionGroupById(item.ClientRolePgid);

              if (oClientRolePermissionGroup != null)
              {
                await _permissionRepository.DeleteClientRolePermissionGroup(oClientRolePermissionGroup);
              }
            }
          }
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
public class AddUpdateClientRolePermissionGroupCommandValidator : AbstractValidator<AddUpdateClientRolePermissionGroupCommand>
{
  public AddUpdateClientRolePermissionGroupCommandValidator()
  {
    RuleFor(x => x.ClientUserRoleId).NotNull().GreaterThan(0);
  }
}
