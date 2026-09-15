using System.Net;
using System.Text.RegularExpressions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Web.Common;

namespace Shipra.Backend.API.Web.Api;

/// <summary>
/// If your API controllers will use a consistent route convention and the [ApiController] attribute (they should)
/// then it's a good idea to define and use a common base controller class like this one.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : Controller
{
  private readonly IServiceProvider _serviceProvider;

  /// <summary>
  /// Initializes a new instance of the <see cref="ApiController"/> class.
  /// </summary>
  /// <param name="serviceProvider">The service provider.</param>
  protected BaseApiController(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  /// <summary>
  /// Gets the mediator.
  /// </summary>
  /// <value>
  /// The mediator.
  /// </value>
  protected IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();

  /// <summary>
  /// Gets the mapper.
  /// </summary>
  /// <value>
  /// The mapper.
  /// </value>
  protected IMapper Mapper => _serviceProvider.GetRequiredService<IMapper>();

  /// <summary>
  /// Gets the feature flags.
  /// </summary>
  /// <value>
  /// The feature flags.
  /// If we decide to start using FeatureFlags.
  /// </value>
  //protected IFeatureFlags FeatureFlags => _serviceProvider.GetRequiredService<IFeatureFlags>();

  /// <summary>
  /// Unwraps the application response model and creates a uniform API response model.
  /// </summary>
  /// <typeparam name="TInput">The result type from the application response <see cref="ObjectResponse{T}"/></typeparam>
  /// <typeparam name="TOutput">The view model type to be returned to the client.</typeparam>
  /// <param name="applicationResult">The application response.</param>
  /// <returns></returns>
  protected IActionResult PrepareResponse<TInput, TOutput>(ObjectResponse<TInput> applicationResult)
      where TOutput : ViewModel
  {
    var result = GetModelResult<TInput, TOutput>(applicationResult.Result!);
    result.Metadata = applicationResult.Metadata;

    return GetResponse(applicationResult.ResultType, result);
  }

  /// <summary>
  /// Unwraps the application response model and creates a uniform API response model.
  /// </summary>
  /// <typeparam name="TInput">The result type from the application response <see cref="ObjectResponse{T}"/></typeparam>
  /// <typeparam name="TOutput">The view model type to be returned to the client.</typeparam>
  /// <param name="applicationResult">The application response.</param>
  /// <returns></returns>
  protected IActionResult PrepareResponse<TInput, TOutput>(ObjectSetResponse<TInput> applicationResult)
      where TOutput : ViewModel
  {
    var viewModelResults = applicationResult.Results!
        .Select(GetModelResult<TInput, TOutput>);

    var set = new ViewModelResultSet<TOutput>(viewModelResults) { Metadata = applicationResult.Metadata };
    return GetResponse(applicationResult.ResultType, set);
  }

  /// <summary>
  /// Unwraps the application response model and creates a uniform API response model.
  /// </summary>
  /// <typeparam name="TInput">The result type from the application response <see cref="ObjectResponse{T}"/></typeparam>
  /// <typeparam name="TOutput">The view model type to be returned to the client.</typeparam>
  /// <param name="applicationResult">The application response.</param>
  /// <returns></returns>
  protected IActionResult PrepareResponse<TInput, TOutput>(PagedObjectSetResponse<TInput> applicationResult)
      where TOutput : ViewModel
  {
    var viewModelResults = applicationResult.Results!
        .Select(GetModelResult<TInput, TOutput>);

    var set = new PagedViewModelResultSet<TOutput>(
        viewModelResults,
        applicationResult.Page,
        applicationResult.PageSize,
        applicationResult.Total);

    return GetResponse(applicationResult.ResultType, set);
  }

  private ViewModelResult<TOutput> GetModelResult<TInput, TOutput>(ResponseObject<TInput> applicationResult)
      where TOutput : ViewModel
  {
    if (applicationResult.Value == null)
      return null!;

    var mapped = Mapper.Map<TInput, TOutput>(applicationResult.Value);
    mapped.Metadata = new Metadata(applicationResult.Metadata);
    var vmResult = new ViewModelResult<TOutput>(mapped);

    return vmResult;
  }

  private IActionResult GetResponse(ApplicationResult resultType, ViewModelResult result = null!)
  {
    HttpStatusCode statusCode;

    switch (resultType)
    {
      case ApplicationResult.Unspecified:
        statusCode = HttpStatusCode.OK;
        break;
      case ApplicationResult.Found:
        statusCode = HttpStatusCode.OK;
        break;
      case ApplicationResult.NotFound:
        statusCode = HttpStatusCode.NotFound;
        break;
      case ApplicationResult.Created:
        statusCode = HttpStatusCode.Created;
        break;
      case ApplicationResult.Ok:
        statusCode = HttpStatusCode.OK;
        break;
      case ApplicationResult.NoAction:
        statusCode = HttpStatusCode.NotAcceptable;
        break;
      case ApplicationResult.Queued:
        statusCode = HttpStatusCode.Accepted;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(resultType), resultType, null);
    }

    return result == null
        ? (IActionResult)new StatusCodeResult((int)statusCode)
        : new StatusCodeObjectResult((int)statusCode, result);
  }

  #region -- Validation --


  protected void EnsureNotNullOrEmpty(string value, string parameterName)
  {
    if (string.IsNullOrEmpty(value))
      throw new ShipraApplicationException(HttpStatusCode.NoContent, parameterName);
  }


  public override void OnActionExecuting(ActionExecutingContext context)
  {
    if (!ModelState.IsValid)
    {
      //var errors = new List<ServiceError>();
      //foreach (var entry in ModelState.Where(k => !string.IsNullOrEmpty(k.Key)))
      //{
      //  errors.AddRange(entry.Value!.Errors.Select(e => new ServiceError
      //  {
      //    Code = 400,
      //    Message = e.ErrorMessage
      //  }));
      //}
      //var response = new ServiceError
      //{
      //  Code = 400,
      //  Message = "Bad Request",
      //  Trace = errors
      //};

      var _errorResponseModel = new ServiceResultDTO();
      var errorFlields = ModelState.ToDictionary(
           kvp => kvp.Key,
           kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
          ).Where(x => x.Value.Length > 0)
          .ToDictionary(kvp => kvp.Key,
           kvp => kvp.Value!.Select(e => e).ToArray());

      _errorResponseModel.Errors = errorFlields;
      _errorResponseModel.IsSuccess = false;
      //_errorResponseModel.Error = "Bad request. One or more validation errors occurred.";
      _errorResponseModel.StatusCode = (int)HttpStatusCode.BadRequest;

      var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
      var actionName = descriptor!.ActionName;
      if ("createorder".Trim().ToLower() == actionName.Trim().ToLower())
      {
        _errorResponseModel.ErrorCombined = combineErrorMessagesByRow(errorFlields);
      }


      context.Result = BadRequest(_errorResponseModel);
    }
  }

  private Dictionary<int, string> combineErrorMessagesByRow(Dictionary<string, string[]> ErrorFields)
  {
    string ErrorMessageFormat = "Please correct the following <br> ";

    Dictionary<int, string> ErrorsCombined = new();

    foreach (var x in ErrorFields)
    {
      string val = x.Key;
      var values = val.Split(".").ToList();
      //assuming three levels

      string pattern = @"\[(\d+)\]";
      Match match = Regex.Match(values[0], pattern);
      int index = 0;


      if (match.Success)
      {
        string indexString = match.Groups[1].Value;
        index = int.Parse(indexString);

        var input = (values.LastOrDefault() == null ? "" : values.LastOrDefault()!);
        string result = input.Replace($"{"Id"}", "");

        if (ErrorsCombined.ContainsKey(index))
        {
          ErrorsCombined[index] = ErrorsCombined[index] + ", " + result;
        }
        else
        {
          ErrorsCombined.Add(index, ErrorMessageFormat + result);
        }

      }

    }
    return ErrorsCombined;

  }

  #endregion
}
