using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.ExcelExportCarrierSettlementById;
using Shipra.Backend.API.Application.Features.TotalProcessFeature.Command.TotalProcessingPrepareTheCheckout;
using Shipra.Backend.API.Application.Features.TotalProcessFeature.Query.FetchPaybylinkbyServiceUUId;
using Shipra.Backend.API.Application.Features.TotalProcessFeature.Query.GetTotalProcessingGetPaymentStatus;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.AddUpdateClientPayoutBank;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.AddUpdateWallet;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.CreatePayoutRequest;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.ExcelExportAllTransaction;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllClientPayoutBank;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllClientWallet;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllOrdersByPayoutId;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllPayoutFileByPayoutId;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllPayouts;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllPayoutStatusHistory;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllTransaction;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetClientPayoutBank;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetPDFAllOrdersByPayoutId;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.GetWalletByClient;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class WalletController : BaseApiController
{
  public WalletController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region command
  [HttpPost("AddUpdateWallet")]
  public async Task<ActionResult> AddUpdateWallet([FromBody] AddUpdateWalletCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("AddUpdateClientPayoutBank")]
  public async Task<ActionResult> AddUpdateClientPayoutBankCommand([FromBody] AddUpdateClientPayoutBankCommand request, CancellationToken cancellationToken = default)
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

  [HttpPost("PrepareTotalProcessingCheckout")]
  public async Task<ActionResult> PrepareTotalProcessingCheckout([FromBody] TotalProcessingPrepareTheCheckoutCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  }

  #endregion
  #region query
  [HttpGet("GetWallet")]
  public async Task<ActionResult> GetWallet(CancellationToken cancellationToken = default)
  {
    GetWalletQuery request = new GetWalletQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetClientPayoutBank")]
  public async Task<ActionResult> GetClientPayoutBank(CancellationToken cancellationToken = default)
  {
    GetClientPayoutBankQuery request = new GetClientPayoutBankQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllWallets")]
  public async Task<ActionResult> GetAllWallets(CancellationToken cancellationToken = default)
  {
    GetAllClientWalletQuery request = new GetAllClientWalletQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllPayoutBanks")]
  public async Task<ActionResult> GetAllPayoutBanks(CancellationToken cancellationToken = default)
  {
    GetAllClientPayoutBankQuery request = new GetAllClientPayoutBankQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllPayouts")]
  public async Task<ActionResult> GetAllPayouts([FromBody] GetAllPayoutsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllPayoutStatusHistory")]
  public async Task<ActionResult> GetAllPayoutStatusHistory([FromBody] GetAllPayoutStatusHistoryQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  
  [HttpGet("GetAllPayoutFileByPayoutId")]
  public async Task<ActionResult> GetAllPayoutFileByPayoutId([FromQuery] GetAllPayoutFileByPayoutIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
   
  [HttpPost("GetAllTransaction")]
  public async Task<ActionResult> GetAllTransaction([FromBody] GetAllTransactionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ExcelExportAllTransaction")]
  public async Task<ActionResult> ExcelExportAllTransaction([FromBody] ExcelExportAllTransactionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("Transactions"));
  } 
  

  [HttpGet("GetTotalProcessingPaymentStatus")]
  public async Task<ActionResult> GetTotalProcessingPaymentStatus([FromQuery] GetTotalProcessingPaymentStatusQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  } 
  [HttpPost("FetchPaybylinkbyServiceUUId")]
  public async Task<ActionResult> FetchPaybylinkbyServiceUUId([FromBody] FetchPaybylinkbyServiceUUIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  }
  [HttpPost("GetPDFAllOrdersByPayoutId")]
  public async Task<ActionResult> GetPDFAllOrdersByPayoutId([FromBody] GetPDFAllOrdersByPayoutIdQuery request, CancellationToken cancellationToken = default)
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
  [HttpPost("GetAllOrdersByPayoutId")]
  public async Task<ActionResult> GetAllOrdersByPayoutId([FromBody] GetAllOrdersByPayoutIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  }
  #endregion
}
