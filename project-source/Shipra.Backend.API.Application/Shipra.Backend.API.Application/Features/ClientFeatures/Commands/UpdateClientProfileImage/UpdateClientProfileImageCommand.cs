using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UploadClientImage;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateClientProfileImage;
public class UpdateClientProfileImageCommand : IRequest<ServiceResultDTO>
{
  public string? ClientImage { get; set; }
}
public class UpdateClientProfileImageCommandHandler : RequestHandlerBase<UpdateClientProfileImageCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public UpdateClientProfileImageCommandHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<UpdateClientProfileImageCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientProfileImageCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientId!.Value);
      }
      client.UpdateClientProfileImage(request.ClientImage, _currentUser.EmployeeId!);
      var data = await _clientRepository.UpdateClient(client);

      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class UpdateClientProfileImageCommandValidator : AbstractValidator<UpdateClientProfileImageCommand>
{
  public UpdateClientProfileImageCommandValidator()
  {
    RuleFor(x => x.ClientImage).NotNull().NotEmpty();
  }

}
