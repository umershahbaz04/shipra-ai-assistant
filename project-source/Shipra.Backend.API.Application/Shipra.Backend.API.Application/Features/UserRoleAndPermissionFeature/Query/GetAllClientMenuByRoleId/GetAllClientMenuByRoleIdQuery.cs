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

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientMenuByRoleId;
public class GetAllClientMenuByRoleIdQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllClientMenuByRoleIdQueryHandler : RequestHandlerBase<GetAllClientMenuByRoleIdQuery, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public GetAllClientMenuByRoleIdQueryHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<GetAllClientMenuByRoleIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientMenuByRoleIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _permissionRepository.GetAllClientMenuByRoleId(_currentUser.ClientId!,_currentUser.RoleId.GetValueOrDefault());
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
