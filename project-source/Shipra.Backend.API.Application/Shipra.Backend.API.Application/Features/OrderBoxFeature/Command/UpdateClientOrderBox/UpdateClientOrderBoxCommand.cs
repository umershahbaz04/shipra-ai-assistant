using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderBoxFeature.Command.UpdateClientOrderBox;
public class UpdateClientOrderBoxCommand : IRequest<ServiceResultDTO>
{
  public int ClientOrderBoxId { get; set; }
  public string? BoxName { get; set; }
  public decimal? Length { get; set; }
  public decimal? Width { get; set; }
  public decimal? Height { get; set; }
  public bool? IsDefault { get; set; }
}
public class UpdateClientOrderBoxCommandHandler : RequestHandlerBase<UpdateClientOrderBoxCommand, ServiceResultDTO>
{
  private readonly IOrderBoxRepository _orderBoxRepository;

  public UpdateClientOrderBoxCommandHandler(IOrderBoxRepository orderBoxRepository, IServiceProvider serviceProvider, ILogger<UpdateClientOrderBoxCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderBoxRepository = orderBoxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientOrderBoxCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClientOrderBox = await _orderBoxRepository.GetClientOrderBoxById(request.ClientOrderBoxId, _currentUser.ClientId!);

      if (oClientOrderBox is null)
      {
        throw new EntityNotFoundException("ClientOrderBox", request.ClientOrderBoxId!);
      }
      decimal? volume = null;
      if (request.Length.HasValue && request.Width.HasValue && request.Height.HasValue)
      {
        // Calculate the volume
        volume = (request.Length.Value * request.Width.Value * request.Height.Value) / ApplicationConstants.WeightDimensionalFactor; //5000 is dimensional factor
      }
      else
      {
        // If any of the dimensions are null, volume cannot be calculated
        volume = null;
      }
      oClientOrderBox.Update(request.BoxName, request.Length, request.Width, request.Height, volume, request.IsDefault, _currentUser.EmployeeId!);
      var isAdded = await _orderBoxRepository.UpdateClientOrderBox(oClientOrderBox);
      if (request!.IsDefault.GetValueOrDefault(false))
      {
        var allOrderBoxes = await _orderBoxRepository.GetAllClientClientOrderBox(_currentUser.ClientId!);
        foreach (var defaultOrderBox in allOrderBoxes)
        {
          if (defaultOrderBox.IsDefault.GetValueOrDefault())
          {
            //remove existing default
            defaultOrderBox.RemoveDefault();
            await _orderBoxRepository.UpdateClientOrderBox(defaultOrderBox);
          }
        }
        //add new default
        oClientOrderBox.MarkDefault();
        await _orderBoxRepository.UpdateClientOrderBox(oClientOrderBox);
      }

      if (isAdded)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Message = "Update Successfully"
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
public class UpdateClientOrderBoxCommandValidator : AbstractValidator<UpdateClientOrderBoxCommand>
{
  public UpdateClientOrderBoxCommandValidator()
  {
    RuleFor(x => x.ClientOrderBoxId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.BoxName).NotEmpty().NotNull();
    RuleFor(x => x.Length).NotEmpty().NotNull();
    RuleFor(x => x.Width).NotEmpty().NotNull();
    RuleFor(x => x.Height).NotEmpty().NotNull();
  }
}
