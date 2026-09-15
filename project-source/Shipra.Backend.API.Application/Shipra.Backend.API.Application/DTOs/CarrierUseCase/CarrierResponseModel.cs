using Newtonsoft.Json;
using Shipra.Backend.API.Application.Helpers;

namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;
public class CarrierResponseDTO
{
  public int CarrierId { get;  set; }
  public string? Name { get;  set; }
  public string? CarrierImage { get;  set; }
  public string? CarrierWebsite { get;  set; }
  public bool? IsClientCarrier { get;  set; }
  public long CountryId { get;  set; } 
  public DateTime? CreatedOn { get;  set; } 
  public DateTime? UpdateOn { get;  set; }
  public bool? Active { get;  set; }
}
public class ActiveCarrierResponseDTO
{
  public int ActiveCarrierId { get;  set; }
  public int CarrierId { get;  set; }
  public string? ClientId { get;  set; }
  public CarrierConfigModel? Config { get;  set; } 
  public DateTime? CreatedOn { get;  set; } 
  public DateTime? UpdatedOn { get;  set; }
  public bool? Active { get;  set; }
}

public class IntegrationCarrierResponseModel<T>
{
  public bool isSuccess { get; set; }
  public int statusCode { get; set; }
  public string? message { get; set; }
  public T? data { get; set; }
  public Dictionary<string, string[]>? errors { get; set; }
  public Dictionary<string, string[]>? configErrors { get; set; }
  public DateTime createdTime { get; set; }
}
public class AwbListResponseModel
{ 
  public List<AwbResponseModel>? streams { get; set; }
}
public class AwbResponseModel
{
  [JsonConverter(typeof(MemoryStreamConverter))]
  public Stream? stream { get; set; }
}

public class Trackingnoswithref
{
  public string? tracking_no { get; set; }
  // barcode is used for order number
  public string? orderNo { get; set; }
  public string? shipper_Ref { get; set; }
}
public class OrderErrorResponseModel
{
  public string? OrderNo { get; set; }
  public string? Messages { get; set; }
}
public class RefreshStatusResponse
{ 
  public string? orderId { get; set; }
  public string? orderNo { get; set; }
  public string? TrackingStatus { get; set; } 
  public int trackingStatusId { get; set; }
  public DateTime? CarrierLastUpdateDateTime { get; set; }
}

#region UploadPaperlessDoc Response
public class PaperlessDocResponse
{
  public bool IsSuccess { get; set; }
  public int StatusCode { get; set; }
  public string? Message { get; set; }
  public Uploadresponse? Data { get; set; }   
  public object? Errors { get; set; }
  public object? ConfigErrors { get; set; }
  public DateTime CreatedTime { get; set; }
}
public class Uploadresponse
{
  public Responsedet? Response { get; set; }
  public Formshistorydocumentid? FormsHistoryDocumentID { get; set; }
}
public class Responsedet
{
  public Responsestatusdet? ResponseStatus { get; set; }
  public Transactionreferenceinfo? TransactionReference { get; set; }
}
public class Responsestatusdet
{
  public string? Code { get; set; }
  public string? Description { get; set; }
}
public class Transactionreferenceinfo
{
  public string? CustomerContext { get; set; }
}
public class Formshistorydocumentid
{
  public string? DocumentID { get; set; }
}
#endregion

#region RaisePickupLocation Response 
public class ResponseStatus
{
  public string? code { get; set; }
  public string? description { get; set; }
}
public class Response
{
  public ResponseStatus? responseStatus { get; set; }
}
public class Data
{
  public Response? response { get; set; }
  public string? prn { get; set; }
  public ResponseStatus? rateStatus { get; set; }
}
public class CreatePickupLocationResponse
{
  public bool isSuccess { get; set; }
  public int statusCode { get; set; }
  public string? message { get; set; }
  public Data? data { get; set; }
  public object? errors { get; set; }
  public object? configErrors { get; set; }
  public DateTime? createdTime { get; set; }
}
#endregion

#region control pane
public class ShipraControlPaneResponseModel<T>
{
  public bool isSuccess { get; set; }
  public int statusCode { get; set; }
  public string? message { get; set; }
  public T? result { get; set; }
  public DateTime createdTime { get; set; }
  public Dictionary<string, string[]>? errors { get; set; }

}

#endregion
