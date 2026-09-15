using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.CreateDriverReceivable;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.CreateExpense;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.DeleteDriverReceivable;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.DeleteExpense;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverExpense;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivable;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivableStatus;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverReceivableStatusUnPaid;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.ExcelExportDriverExpense;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetAllDriverExpense;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetAllDriverReceivable;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverExpenseById;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverReceivableById;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverAccountBalanceById;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.ExportDriverReceivables;
using Shipra.Backend.API.Application.Helpers;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class DriverAccountController : BaseApiController
{
  public DriverAccountController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }


  #region Expense
  #region command
  [HttpPost("CreateExpense")]
  public async Task<ActionResult> CreateExpense([FromBody] CreateExpenseCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateExpense")]
  public async Task<ActionResult> UpdateExpense([FromBody] UpdateExpenseCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteExpense")]
  public async Task<ActionResult> DeleteExpense([FromBody] DeleteExpenseCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region query
  [HttpPost("GetAllDriverExpense")]
  public async Task<ActionResult> GetAllDriverExpense([FromBody] GetAllDriverExpenseQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetExpenseById")]
  public async Task<ActionResult> GetExpenseById([FromQuery] GetExpenseByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("ExcelExportDriverExpense")]
  public async Task<ActionResult> ExcelExportExpense([FromBody] ExcelExportDriverExpenseQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("Expense"));

  }
  #endregion 
  #endregion
  #region DriverReceivable
  #region command
  [HttpPost("CreateDriverReceivable")]
  public async Task<ActionResult> CreateDriverReceivable([FromBody] CreateDriverReceivableCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateDriverReceivable")]
  public async Task<ActionResult> UpdateDriverReceivable([FromBody] UpdateDriverReceivableCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteDriverReceivable")]
  public async Task<ActionResult> DeleteDriverReceivable([FromBody] DeleteDriverReceivableCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateDriverReceivableStatusPaid")]
  public async Task<ActionResult> UpdateDriverReceivableStatusPaid([FromBody] UpdateDriverReceivableStatusPaidCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateDriverReceivableStatusUnPaid")]
  public async Task<ActionResult> UpdateDriverReceivableStatusUnPaid([FromBody] UpdateDriverReceivableStatusUnPaidCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region query
  [HttpPost("GetAllDriverReceivable")]
  public async Task<ActionResult> GetAllDriverReceivable([FromBody] GetAllDriverReceivableQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetDriverReceivableById")]
  public async Task<ActionResult> GetDriverReceivableById([FromQuery] GetDriverReceivableByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetDriverAccountBalanceById")]
  public async Task<ActionResult> GetDriverAccountBalanceById([FromBody] GetDriverAccountBalanceByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("ExportDriverReceivables")]
  public async Task<ActionResult> ExportDriverReceivables([FromBody] ExportDriverReceivablesQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion 
  #endregion
}
