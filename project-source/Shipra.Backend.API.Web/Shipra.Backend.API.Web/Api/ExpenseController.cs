using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ExpenseFeatures.Command.CreateExpenseCategory;
using Shipra.Backend.API.Application.Features.ExpenseFeatures.Query.GetAllExpense;
using Shipra.Backend.API.Application.Features.ExpenseFeatures.Query.GetAllMyCarrierExpense;
using Shipra.Backend.API.Application.Features.ExpenseFeatures.Query.GetPDFDriverExpenseReport;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class ExpenseController : BaseApiController
{
  public ExpenseController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("CreateExpenseCategory")]
  public async Task<ActionResult> CreateExpenseCategory([FromBody] CreateExpenseCategoryCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region query
  [HttpPost("GetAllMyCarrierExpenseQuery")]
  public async Task<ActionResult> GetAllMyCarrierExpenseQuery([FromBody] GetAllMyCarrierExpenseQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllExpenseAccount")]
  public async Task<ActionResult> GetAllExpenseAccount([FromBody] GetAllExpenseAccountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetPDFDriverExpenseReport")]
  public async Task<ActionResult> GetPDFDriverExpenseReport([FromBody] GetPDFDriverExpenseReportQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("ExpenseReport");
    try
    {
      return File(result.Result!, PDFDocumentGenerator.ContentType, file);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
    return Ok(result);
  }

  #endregion
}
