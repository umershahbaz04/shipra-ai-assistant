using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.Drawing.Slicer.Style;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.DTOs.AccountUserCase;
public class UDTCarrierSettlementSimplified
{
  public string? OrderNo { get; set; }
  public string? PaymentRef { get; set; }
  public decimal? FileAmount { get; set; }
  public DateTime? PaymentDate { get; set; }

  public static UDTCarrierSettlementDetailResponse ConvertoCarrierSettlementDetail(IList<UDTCarrierSettlementSimplified> dataList, List<Order> orders, int carrierId)
  {
    UDTCarrierSettlementDetailResponse response = new UDTCarrierSettlementDetailResponse();
    List<CarrierSettlementResponseModel> shipments = new List<CarrierSettlementResponseModel>();
    List<UDTFileUploadError> errors = new List<UDTFileUploadError>();

    int i = 0;
    foreach (var x in dataList)
    {
      i++;
      var obj = new CarrierSettlementResponseModel();
      UDTFileUploadError error = new UDTFileUploadError();

      #region Index 
      error.Row = i;
      error.IsSuccessed = true;
      #endregion
      #region Amount
      if (x.FileAmount > 0)
      {
        obj.FileAmount = x.FileAmount;
      }
      else
      {
        obj.FileAmount = 0;
      }
      #endregion

      #region OrderNo
      if (!string.IsNullOrEmpty(x.OrderNo))
      {
        obj.OrderNo = x.OrderNo;
        var order = orders.FirstOrDefault(y => y.OrderNo == x.OrderNo);

        
        if(order != null)
        {
          if (order!.CarrierId is not null)
          {
            if (order.CarrierId != carrierId)
            {
              error.IsSuccessed = false;
              error.Msg.Add("The Order not found against the selected carrier.");
            }
          }
          else
          {
            error.IsSuccessed = false;
            error.Msg.Add("The Order does not have a carrier.");
          }
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Order not found.");
        }

        #region CarrierTrackingNo
        if (!string.IsNullOrEmpty(order!.CarrierTrackingNo))
        {
          obj.CarrierTrackingNo = order!.CarrierTrackingNo;
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("Carrier Tracking No is Missing.");
        }
        #endregion

        obj.PaymentMethodId = order.PaymentMethodId;
        obj.OrderType = Enum.GetName(typeof( EnumOrderType), order.OrderTypeId!);
        obj.DbAmount = order.Amount;
        obj.Diffrence = x.FileAmount - order.Amount;
        obj.PaymentStatus = Enum.GetName(typeof( EnumPaymentStatus), order.PaymentStatusId!);
        obj.PaymentStatusId = order.PaymentStatusId;
        if (order.CarrierPaymentSettlementId is not null)
        {
          error.IsSuccessed = false;
          error.Msg.Add("Already setteledup.");
        }
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Order No is Missing.");
      }
      #endregion
      #region PaymentRef
      if (!string.IsNullOrEmpty(x.PaymentRef))
      {
        obj.PaymentRef = x.PaymentRef;
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("PaymentRef is Missing.");
      }
      obj.PaymentDate = x.PaymentDate; 
      #endregion

      errors.Add(error);
      shipments.Add(obj);
    }
    response.IsSuccessed = errors.All(x => x.IsSuccessed);
    response.Detail = shipments;


    response.Errors = errors;
    return response;
  }

}

