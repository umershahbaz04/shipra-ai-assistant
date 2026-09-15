using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetFullfilableOrderFile;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.CreateBulkStoreWithSalePerson;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.CreateStore;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.DeleteStoreCommand;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.EnableStoreCommand;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.GenerateEncryptedKeyAgainstStoreAndStation;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.UpdateStoreCommand;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.UploadStore;
using Shipra.Backend.API.Application.Features.StoreFeatures.Command.UploadStoreImage;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetAllStoresQuery;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetNextStoreCode;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetSampleExcelFileForStoreUpload;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoreByIdQuery;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoresFroSelection;

namespace Shipra.Backend.API.Web.Api;

[Authorize]
//[ServiceFilter(typeof(CustomAuthorizationAttribute))]
public class StoreController : BaseApiController
{
  public StoreController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region Commmand 
  [HttpPost("CreateStore")]
  public async Task<ActionResult> CreateStore([FromBody] CreateStoreCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);

  }
  [HttpPost("UpdateStore")]
  public async Task<ActionResult> UpdateStore([FromBody] UpdateStoreCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteStoreById")]
  public async Task<ActionResult> DeleteStoreById([FromBody] DeleteStoreCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GenerateEncryptedKeyAgainstStoreAndStation")]
  public async Task<ActionResult> GenerateEncryptedKeyAgainstStoreAndStation([FromBody] GenerateEncryptedKeyAgainstStoreAndStationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("EnableStoreById")]
  public async Task<ActionResult> EnableStoreById([FromBody] EnableStoreCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UploadStoreImage")]
  public async Task<ActionResult> UploadStoreImage([FromForm] UploadStoreImageCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UploadStoreFile")]
  public async Task<ActionResult> UploadStoreFile([FromForm] UploadStoreCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateBulkStore")]
  public async Task<ActionResult> CreateBulkStore([FromBody] CreateBulkStoreWithSalePersonCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetSampleExcelFileForStoreUpload")]
  public async Task<ActionResult> GetSampleExcelFileForStoreUpload([FromQuery] GetSampleExcelFileForStoreUploadQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region query
  [HttpGet("GetStoreById")]
  public async Task<ActionResult> GetStoreById([FromQuery] GetStoreByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllStores")]
  public async Task<ActionResult> GetAllStores([FromBody] GetAllStoresQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetStoresForSelection")]
  public async Task<ActionResult> GetStoresForSelection()
  {
    var request = new GetStoresForSelectionQuery();
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }

  [HttpGet("GetNextStoreCode")]
  public async Task<ActionResult> GetNextStoreCode()
  {
    var request = new GetNextStoreCodeQuery();
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }
  #endregion 
}

