using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderBoxFeature.Command.EnableDisabpleClientOrderBox;
public class EnableDisableClientOrderBoxCommand : IRequest<ServiceResultDTO>
{
  public int ClientOrderBoxId { get; set; }
  public bool? IsActive { get; set; }
}
public class EnableDisabpleClientOrderBoxCommandHandler : RequestHandlerBase<EnableDisableClientOrderBoxCommand, ServiceResultDTO>
{
  private readonly IOrderBoxRepository _orderBoxRepository;

  public EnableDisabpleClientOrderBoxCommandHandler(IOrderBoxRepository orderBoxRepository, IServiceProvider serviceProvider, ILogger<EnableDisabpleClientOrderBoxCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderBoxRepository = orderBoxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(EnableDisableClientOrderBoxCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClientOrderBox = await _orderBoxRepository.GetClientOrderBoxById(request.ClientOrderBoxId, _currentUser.ClientId!);

      if (oClientOrderBox is null)
      {
        throw new EntityNotFoundException("ClientOrderBox", request.ClientOrderBoxId!);
      }

      oClientOrderBox.EnableDisable(request.IsActive, _currentUser.EmployeeId!);
      var isAdded = await _orderBoxRepository.UpdateClientOrderBox(oClientOrderBox);
      if (isAdded)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Message = "Deleted Successfully"
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
public class EnableDisableClientOrderBoxCommandValidator : AbstractValidator<EnableDisableClientOrderBoxCommand>
{
  public EnableDisableClientOrderBoxCommandValidator()
  {
    RuleFor(x => x.ClientOrderBoxId).NotEmpty().NotNull();
    RuleFor(x => x.IsActive).NotEmpty().NotNull(); 
  } 
}
