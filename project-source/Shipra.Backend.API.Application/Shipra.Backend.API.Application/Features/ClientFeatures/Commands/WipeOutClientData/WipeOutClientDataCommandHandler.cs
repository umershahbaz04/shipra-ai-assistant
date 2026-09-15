using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.WipeOutClientData;

public class WipeOutClientDataCommandHandler : RequestHandlerBase<WipeOutClientDataCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IClientRepository _clientRepository;

  public WipeOutClientDataCommandHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<WipeOutClientDataCommand> logger) 
    : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(WipeOutClientDataCommand request, CancellationToken cancellationToken)
  {
    try
    {
      if (_currentUser == null || _currentUser.ClientId == null || _currentUser.RoleId != (int)EnumUserRole.Admin)
      {
        throw new Exception("Unauthorized or client session not found.");
      }

      var sectionEnum = Shipra.Backend.API.Core.Enum.EnumWipeOutSection.All;
      if (!string.IsNullOrEmpty(request.Section))
      {
        Enum.TryParse(request.Section, true, out sectionEnum);
      }

      var isSuccess = await _clientRepository.WipeOutClientData(_currentUser.ClientId, sectionEnum);
      
      var resultModel = new BaseResponseDto 
      { 
        Data = isSuccess, 
        Message = isSuccess ? "Client data wiped out successfully." : "Failed to wipe out client data." 
      };

      var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>(resultModel);
      response.IsSuccess = isSuccess;

      return response;
    }
    catch (Exception ex)
    {
      var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
