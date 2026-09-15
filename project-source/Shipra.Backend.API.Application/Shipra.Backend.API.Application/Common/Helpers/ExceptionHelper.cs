using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Application.Services.Interfaces;

namespace Shipra.Backend.API.Application.Common.Helpers;

public class ExceptionHelper : IExceptionHelper
{

  public ExceptionHelper(IEmailHandler emailHandler,
    //IUnitOfWork unitOfWork,
    IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
  {
    _configuration = configuration;
    _emailHandler = emailHandler;
    //_unitOfWork = unitOfWork;
    _hostingEnvironment = hostingEnvironment;

  }
  private readonly IWebHostEnvironment _hostingEnvironment;
  private readonly IEmailHandler _emailHandler;

  private IConfiguration _configuration { get; }
  //private IUnitOfWork _unitOfWork { get; }

  public ErrorResponse GetErrorResponse(Exception ex,string tenantId, bool sendEmail = false, string? customMessage = null!, bool logToDb = true)
  {
    string errorMessage = null!;
    var errorId = 0;
    Dictionary<string, string[]> errDict = new Dictionary<string, string[]>();
    if (logToDb)
    {
      errorId = Log(ex, sendEmail: sendEmail);
    }

    //if no custom message then add the error log //  in case of Exception type
    if (string.IsNullOrWhiteSpace(customMessage))
    {
      errorMessage = $"Something went wrong on the server, Please reach out to support and refer to this errorId: {errorId}";
      errDict.Add("Error", new[] { errorMessage });
    }
    else //else add the custom message
    {
      errorMessage = customMessage;
      errDict.Add("Error", new[] { errorMessage }); 
    }
    return new ErrorResponse { Errors = errDict, IsSuccess = false, ErrorID = errorId };
  }

  public int Log(Exception e, bool sendEmail = false)
  {
    try
    {
      //if (configuration != null && configuration["AppEnvironment"] == "dev")
      //{
      //    sendEmail = false;
      //}

      //var options = ThreeTasty.Infrastructure.Common.Common.options;

      //var context = new WaitLess.Core.Domain.Data.WaitLessDbContext;

      ////var _errorlogRepository = new GenericRepository<ErrorLog>(context);

      var Description = string.Empty;

      if (!string.IsNullOrEmpty(Convert.ToString(e.InnerException)))
      {
        Description = " InnerException: " + e.InnerException;
      }
      if (!string.IsNullOrEmpty(Convert.ToString(e.Message)))
      {
        if (!string.IsNullOrEmpty(Description))
        {
          Description += Environment.NewLine;
        }
        Description += " Message: " + e.Message;
      }
      if (!string.IsNullOrEmpty(Convert.ToString(e.StackTrace)))
      {
        if (!string.IsNullOrEmpty(Description))
        {
          Description += Environment.NewLine;
        }
        Description += " StackTrace: " + e.StackTrace;
      }
      if (!string.IsNullOrEmpty(Convert.ToString(_hostingEnvironment.EnvironmentName)))
      {
        if (!string.IsNullOrEmpty(Description))
        {
          Description += Environment.NewLine;
        }
        Description += " Environment: " + _hostingEnvironment.EnvironmentName;
      }
      //var errorLog = new ErrorLog
      //{
      //  ErrorText = Description,
      //  ErrorType = nameof(ExceptionType.Core),
      //  CreatedDate = DateTime.UtcNow,
      //  CreatedBy = "1"
      //};

      //_unitOfWork.ErrorLog.Add(errorLog);
      //_unitOfWork.Complete();

      //TODO: replace with simpler emails
      if (sendEmail && _configuration.GetValue<bool>("EmailConfiguration:SendMail"))
      {
        //zeeshan.adil@waitless-medical.com
        var emailSubject = "Erorr Occured on the project";
        var emailContent = "Dear Admin ,<br/><br/>" + Description + "<br/><br/>" + "Regards <br/>Support Team";

        //Sending Email
        bool ret = _emailHandler.SendEmail(emailSubject, emailContent, "z.adil@shipra.com", "");

        //var _emailSender = new EmailSender();
      }

      // return errorLog.Id;
      return 0;
    }
    catch
    {
      throw;
    }
  }
}
