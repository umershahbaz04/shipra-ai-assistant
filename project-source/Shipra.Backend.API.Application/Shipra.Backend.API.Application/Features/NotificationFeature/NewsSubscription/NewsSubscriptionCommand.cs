using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Services.Interfaces;
using FluentValidation;

namespace Shipra.Backend.API.Application.Features.NotificationFeature.NewsSubscription;

public class NewsSubscriptionCommand : IRequest<ServiceResultDTO>
{
  public string? RecipientEmail { get; set; }
}
public class ContactUsCommandHandler : RequestHandlerBase<NewsSubscriptionCommand, ServiceResultDTO>
{ 
  public ContactUsCommandHandler( IServiceProvider serviceProvider, ILogger<ContactUsCommandHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(NewsSubscriptionCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _emailServiceProvider.SendEmailForNewsSubscription(request.RecipientEmail);

      if (result)
      {
        serviceResult = new ServiceResultDTO(new { Message = "Email send successfully!" });
      }
      else
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
        serviceResult.Errors!.Add("Error", new string[] { "Something went wrong!" });
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
public class ContactUsCommandValidator : AbstractValidator<NewsSubscriptionCommand>
{
  public ContactUsCommandValidator()
  {
    RuleFor(x => x.RecipientEmail).NotEmpty().NotNull();
  }
}
