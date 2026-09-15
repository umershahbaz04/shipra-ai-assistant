using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.LeadFeatures.Command;
using Shipra.Backend.API.Application.Features.LeadFeatures.Query;
using Shipra.Backend.API.Application.Features.LeadFeatures.Query.ExcelExportAllLeads;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Web.Api;

[Authorize]
public class LeadController : BaseApiController
{
    public LeadController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    #region command

    [HttpPost("UploadLeads")]
    public async Task<ActionResult> UploadLeads([FromForm] UploadLeadsCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("UpdateLeadStatus")]
    public async Task<ActionResult> UpdateLeadStatus([FromBody] UpdateLeadStatusCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("AssignSalesperson")]
    public async Task<ActionResult> AssignSalesperson([FromBody] AssignSalespersonCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("CreateBulkLeads")]
    public async Task<ActionResult> CreateBulkLeads([FromBody] CreateBulkLeadsCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("UpdateLead")]
    public async Task<ActionResult> UpdateLead([FromBody] UpdateLeadCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("UpdateOrderIds")]
    public async Task<ActionResult> UpdateOrderIds([FromBody] UpdateOrderIdsCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DeleteBulkLeads")]
    public async Task<ActionResult> DeleteBulkLeads([FromBody] DeleteBulkLeadsCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    // Client Lead Status CRUD
    [HttpPost("CreateClientLeadStatus")]
    public async Task<ActionResult> CreateClientLeadStatus([FromBody] CreateClientLeadStatusCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("UpdateClientLeadStatus")]
    public async Task<ActionResult> UpdateClientLeadStatus([FromBody] UpdateClientLeadStatusCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("ActiveDeactiveClientLeadStatus")]
    public async Task<ActionResult> ActiveDeactiveClientLeadStatus([FromBody] ActiveDeactiveClientLeadStatusCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    // Lead Tab Column Management (mirrors CreateShipmentGridColumn / UpdateShipmentTabDisplayOrder)
    [HttpPost("CreateLeadGridColumn")]
    public async Task<ActionResult> CreateLeadGridColumn([FromBody] CreateLeadGridColumnCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("UpdateLeadTabDisplayOrder")]
    public async Task<ActionResult> UpdateLeadTabDisplayOrder([FromBody] UpdateLeadTabDisplayOrderCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("UpdateLeadGridColumn")]
    public async Task<ActionResult> UpdateLeadGridColumn([FromBody] UpdateLeadGridColumnCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("DeleteLeadGridColumn")]
    public async Task<ActionResult> DeleteLeadGridColumn([FromBody] DeleteLeadGridColumnCommand request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    #endregion

    #region query

    [HttpPost("GetAllLeads")]
    public async Task<ActionResult> GetAllLeads([FromBody] GetAllLeadsQuery request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("ExcelExportLeads")]
    public async Task<ActionResult> ExcelExportLeads([FromBody] ExcelExportAllLeadsQuery request, CancellationToken cancellationToken = default)
    {
        var response = (ServiceResultDTOWithTypeModel<ExcelResponseModel>?)await Mediator.Send(request, cancellationToken);
        return File(response!.Result?.Bytes!, Shipra.Backend.API.Application.Helpers.ExcelExportHelper.ExcelContentType, Shipra.Backend.API.Application.Helpers.ExcelExportHelper.GetExcelFileName("Leads"));
    }

    [HttpPost("GetAllLeadTabsCount")]
    public async Task<ActionResult> GetAllLeadTabsCount([FromBody] GetAllLeadTabsCountQuery request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("GetSalesPersonLeadStats")]
    public async Task<ActionResult> GetSalesPersonLeadStats([FromBody] GetSalesPersonLeadStatsQuery request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("GetLeadTabsCountConfig")]
    public async Task<ActionResult> GetLeadTabsCountConfig(CancellationToken cancellationToken = default)
    {
        GetLeadTabsCountConfigQuery request = new();
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("GetAllLeadStatusForSelection")]
    public async Task<ActionResult> GetAllLeadStatusForSelection([FromQuery] GetAllLeadStatusForSelectionQuery request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("GetAllClientLeadStatusForSelection")]
    public async Task<ActionResult> GetAllClientLeadStatusForSelection([FromQuery] GetAllClientLeadStatusForSelectionQuery request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }


    [HttpPost("GetLeadContacts")]
    public async Task<ActionResult> GetLeadContacts([FromBody] GetLeadContactsQuery request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("GetOrdersByContactMobile")]
    public async Task<ActionResult> GetOrdersByContactMobile([FromBody] GetOrdersByContactMobileQuery request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("GetAllClientLeadStatus")]
    public async Task<ActionResult> GetAllClientLeadStatus([FromBody] GetAllClientLeadStatusQuery request, CancellationToken cancellationToken = default)
    {
        var response = await Mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    #endregion
}
