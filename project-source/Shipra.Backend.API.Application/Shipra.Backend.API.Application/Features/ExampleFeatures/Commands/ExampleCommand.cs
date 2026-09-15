using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;


namespace Shipra.Backend.API.Application.Features.ExampleFeatures.Commands;

public class ExampleCommand :  IRequest<ServiceResultDTO>
{
  public string? Name { get; set; }
  public string? company { get; set; }

  public class ExampleCommandHandler : RequestHandlerBase<ExampleCommand, ServiceResultDTO>
  {
    private readonly IMediator _mediator;

    public ExampleCommandHandler(IMediator mediator, IServiceProvider serviceProvider, ILogger<ExampleCommandHandler> logger) : base(serviceProvider, logger)
    {
      _mediator = mediator;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(ExampleCommand request, CancellationToken cancellationToken)
    {
      ServiceResultDTO serviceResult = new ServiceResultDTO();
      try
      { 
        await Task.Delay(1);
        return serviceResult;
      }
      catch (Exception ex)
      {
        serviceResult.CreateErrorResponse(ex);
        throw;
      }

    }
  }
  public class ExampleCommandValidator : AbstractValidator<ExampleCommand>
  {
    public ExampleCommandValidator()
    {
      RuleFor(e => e.Name).NotEmpty();
    }
  }
}
