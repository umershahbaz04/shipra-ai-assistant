using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.CreateClientUserRole;
public class CreateClientUserRoleCommand : IRequest<ServiceResultDTO>
{
  public string? RoleName { get; set; }
  public string? RoleDescription { get; set; }
}
public class CreateClientUserRoleCommandHandler : RequestHandlerBase<CreateClientUserRoleCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public CreateClientUserRoleCommandHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<CreateClientUserRoleCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientUserRoleCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      ClientUserRole clientUserRole = ClientUserRole.Create(request.RoleName, request.RoleDescription, _currentUser.ClientId!, false);
      await _permissionRepository.CreateClientUserRole(clientUserRole);

      if (clientUserRole.ClientUserRoleId > 0)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = clientUserRole.ClientUserRoleId,
          Message = "Role created successfully"
        });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class CreateClientUserRoleCommandValidator : AbstractValidator<CreateClientUserRoleCommand>
{
  public CreateClientUserRoleCommandValidator()
  {
    RuleFor(x => x.RoleName).NotEmpty().NotNull(); 
  }
}
