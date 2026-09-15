using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Services.Interfaces;
using FluentValidation;
using Shipra.Backend.API.Application.Common.Constants;

namespace Shipra.Backend.API.Application.Features.NotificationFeature.ContactUs;
public class ContactUsCommand : IRequest<ServiceResultDTO>
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; }
  public string? Phone { get; set; }
  public string? Message { get; set; }
  public string? PrimaryNeed { get; set; }
  public string? MonthlyOrder { get; set; }
  public string? CompanyName { get; set; }
}
public class ContactUsCommandHandler : RequestHandlerBase<ContactUsCommand, ServiceResultDTO>
{ 

  public ContactUsCommandHandler(IServiceProvider serviceProvider, ILogger<ContactUsCommandHandler> logger) : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ContactUsCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var name = request.FirstName + " " + request.LastName;

      string body = "<div style='line-height:inherit;font-family:Avenir,Helvetica,sans-serif;box-sizing:border-box;direction:ltr;text-align:left;'>";
      body += "<br style='line-height:inherit;'><b style='font-size: 16px;'>Contact Information</b>";
      body += "<br style='line-height:inherit;'><b>Name:</b> " + name;
      body += "<br style='line-height:inherit;'><b>Email Address:</b> " + request.Email;
      body += "<br style='line-height:inherit;'><b>Message:</b> " + request.Message;
      body += "<br style='line-height:inherit;'><b>PrimaryNeed:</b> " + request.PrimaryNeed;
      body += "<br style='line-height:inherit;'><b>MonthlyOrder:</b> " + request.MonthlyOrder;
      body += "<br style='line-height:inherit;'><b>CompanyName:</b> " + request.CompanyName;
      body += "<br style='line-height: inherit;'>";
      body += "<b>Shipra</b> App <br style = 'line-height: inherit;' >";
      body += "<b>Note:</b> This is an electronic message.Please do not reply to this email.</div>";

      var fullBody = "<p>" + body + "</p>";

      var result = await _emailServiceProvider.SendExceptionEmailAsync("Contact Information", fullBody);

      if (result)
      {
        serviceResult = new ServiceResultDTO(new { Message = "Email send successfully!"});
      }
      else
      {
        serviceResult.IsSuccess = false;
        serviceResult.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
        serviceResult.Errors!.Add("Error",new string[] {"Something went wrong!"});
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
public class ContactUsCommandValidator : AbstractValidator<ContactUsCommand>
{
  public ContactUsCommandValidator()
  {
    RuleFor(x => x.FirstName).NotEmpty().NotNull();
    RuleFor(x => x.LastName).NotEmpty().NotNull();
    RuleFor(x => x.Email).NotEmpty().NotNull();
    RuleFor(x => x.Phone).NotEmpty().NotNull();
    RuleFor(x => x.Message).NotEmpty().NotNull();
  }
}
