using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Features.TotalProcessFeature.Command.GeneratePaymentLink;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.AdminUpdatePayoutStatus;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.CreatePayoutRequest;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.UpdatePaymentProcessingCharges;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.UpdatePayoutTransactionCharges;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllOrdersByPayoutId;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetPDFAllOrdersByPayoutId;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;
public class PayoutController : BaseApiController
{
  public PayoutController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region command
  [HttpPost("UpdatePayoutStatus")]
  public async Task<ActionResult> UpdatePayoutStatus([FromBody] AdminUpdatePayoutStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreatePayoutRequest")]
  public async Task<ActionResult> CreatePayoutRequest([FromBody] CreatePayoutRequestCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  [HttpPost("UpdatePayoutTransactionCharges")]
  public async Task<ActionResult> UpdatePayoutTransactionCharges([FromBody] UpdatePayoutTransactionChargesCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpPost("UpdateClientPaymentProcessingCharges")]
  public async Task<ActionResult> UpdateClientPaymentProcessingCharges([FromBody] UpdateClientPaymentProcessingChargesCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  [HttpPost("GenerateTotalProcessPaymentLink")]
  public async Task<ActionResult> GeneratePaymentLink([FromBody] GenerateTotalProcessPaymentLinkCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetPDFAllOrdersByPayoutIdAdmin")]
  public async Task<ActionResult> GetPDFAllOrdersByPayoutIdAdmin([FromBody] GetPDFAllOrdersByPayoutIdQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("payoutOrders");
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
  [HttpPost("GetAllOrdersByPayoutIdAdmin")]
  public async Task<ActionResult> GetAllOrdersByPayoutIdAdmin([FromBody] GetAllOrdersByPayoutIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
