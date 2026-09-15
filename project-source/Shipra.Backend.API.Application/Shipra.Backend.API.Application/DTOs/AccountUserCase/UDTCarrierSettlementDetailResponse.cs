using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;

namespace Shipra.Backend.API.Application.DTOs.AccountUserCase;
public class UDTCarrierSettlementDetailResponse
{
  public IList<CarrierSettlementResponseModel>? Detail { get; set; }
  public bool IsSuccessed { get; set; }
  public IList<UDTFileUploadError>? Errors { get; set; }
}
public class CarrierSettlementResponseModel
{
  public string? OrderNo { get; set; }
  public string? CarrierTrackingNo { get; set; }
  public decimal? FileAmount { get; set; }
  public int? PaymentMethodId { get; set; }
  public decimal? DbAmount { get; set; }
  public decimal? Diffrence { get; set; }
  public DateTime? PaymentDate { get; set; }
  public string? PaymentRef { get; set; }
  public string? PaymentStatus { get; set; }
  public string? OrderType { get; set; }
  public int? PaymentStatusId { get; set; }
}
