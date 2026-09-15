using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.AddUpdateTableConfiguration;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.CreateEmployee;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.DeleteEmployee;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.GetNextEmployeeUserName;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.UpdateEmployee;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.UploadEmployeeImage;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllEmployeeColumnConfiguration;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllEmployees;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllEmployeesForSelection;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllEmployeeType;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllGenderForSelection;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllSalePersons;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetEmployeeById;
using Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetEmployeeProfileById;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSalePersonForSelection;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class EmployeeController : BaseApiController
{
  public EmployeeController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("CreateEmployee")]
  public async Task<ActionResult> CreateEmployee([FromBody] CreateEmployeeCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateEmployee")]
  public async Task<ActionResult> UpdateEmployee([FromBody] UpdateEmployeeCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UploadEmployeeImage")]
  public async Task<ActionResult> UploadEmployeeImage([FromForm] UploadEmployeeImageCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("EnableDisableEmploye")]
  public async Task<ActionResult> EnableDisableEmploye([FromQuery] EnableDisableEmployeeCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpPost("AddUpdateTableConfiguration")]
  public async Task<ActionResult> AddUpdateTableConfiguration([FromBody] AddUpdateTableConfigurationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region query
  [HttpPost("GetAllEmployees")]
  public async Task<ActionResult> GetAllEmployees([FromBody] GetAllEmployeesQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllEmployeeColumnConfiguration")]
  public async Task<ActionResult> GetAllEmployeeColumnConfiguration(CancellationToken cancellationToken = default)
  {
    GetAllEmployeeColumnConfigurationQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllSalePersons")]
  public async Task<ActionResult> GetAllSalePersons(CancellationToken cancellationToken = default)
  {
    GetAllSalePersonsQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllEmployeeType")]
  public async Task<ActionResult> GetAllEmployeeType(CancellationToken cancellationToken = default)
  {
    GetAllEmployeeTypeQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetEmployeeProfileById")]
  public async Task<ActionResult> GetEmployeeProfileById([FromQuery] GetEmployeeProfileByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetEmployeeById")]
  public async Task<ActionResult> GetEmployeeById([FromQuery] GetEmployeeByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllEmployeesForSelection")]
  public async Task<ActionResult> GetAllEmployeesForSelection(CancellationToken cancellationToken = default)
  {
    GetAllEmployeesForSelectionQuery request = new GetAllEmployeesForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetGenderForSelection")]
  public async Task<ActionResult> GetGenderForSelection(CancellationToken cancellationToken = default)
  {
    GetGenderForSelectionQuery request = new GetGenderForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetNextEmployeeUserName")]
  public async Task<ActionResult> GetNextEmployeeUserName([FromBody] GetNextEmployeeUserNameQuery request,CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetAllSalePersonForSelection")]
  public async Task<ActionResult> GetAllSalePersonForSelection(CancellationToken cancellationToken = default)
  {
    GetAllSalePersonForSelectionQuery request = new GetAllSalePersonForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
