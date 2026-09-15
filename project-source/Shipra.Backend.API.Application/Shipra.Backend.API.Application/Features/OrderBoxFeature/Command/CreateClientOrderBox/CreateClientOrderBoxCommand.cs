using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderBoxFeature.Command.CreateClientOrderBox;
public class CreateClientOrderBoxCommand : IRequest<ServiceResultDTO>
{
  public string? BoxName { get; set; }
  public decimal? Length { get; set; }
  public decimal? Width { get; set; }
  public decimal? Height { get; set; }
  public bool? IsDefault { get; set; }
}
public class CreateClientOrderBoxCommandHandler : RequestHandlerBase<CreateClientOrderBoxCommand, ServiceResultDTO>
{
  private readonly IOrderBoxRepository _orderBoxRepository;

  public CreateClientOrderBoxCommandHandler(IOrderBoxRepository orderBoxRepository, IServiceProvider serviceProvider, ILogger<CreateClientOrderBoxCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderBoxRepository = orderBoxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientOrderBoxCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      bool isExist = await _orderBoxRepository.IsOrderBoxExist(request.BoxName!, _currentUser.ClientId!);

      if (!isExist)
      {
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

        ClientOrderBox clientOrderBox = ClientOrderBox.Create(_currentUser.ClientId, request.BoxName, request.Length, request.Width, request.Height, volume, request.IsDefault, _currentUser.EmployeeId!);
        var existOClientTax = await _orderBoxRepository.CreateClientOrderBox(clientOrderBox);


        //Check if at the time of create Product station user's select mark as defaul or not?
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
          clientOrderBox.MarkDefault();
          await _orderBoxRepository.UpdateClientOrderBox(clientOrderBox);
        }

        if (existOClientTax is null)
        {
          serviceResult = new ServiceResultDTO(new BaseResponseDto
          {
            Message = "Created Successfully"
          });
        }
      }
      else
      {
        serviceResult.CreateError("AlreadyExist", new string[] { $"Box with name {request.BoxName} is already exist" });
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

public class CreateClientOrderBoxCommandValidator : AbstractValidator<CreateClientOrderBoxCommand>
{
  public CreateClientOrderBoxCommandValidator()
  {
    RuleFor(x => x.BoxName).NotEmpty().NotNull();
    RuleFor(x => x.Length).NotEmpty().NotNull();
    RuleFor(x => x.Width).NotEmpty().NotNull();
    RuleFor(x => x.Height).NotEmpty().NotNull();
  }
}
