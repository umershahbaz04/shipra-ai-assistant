using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.DTOs.ReturnReportUseCase;
public class UDTReturnReportSimplified
{
  public string? OrderNo { get; set; }
  public string? TrackingNo { get; set; }

  public static UDTReturnReportDetailResponse ConvertoReturnReportDetail(IList<UDTReturnReportSimplified> excelDataList, List<dynamic> orderList, int carrierId)
  {
    UDTReturnReportDetailResponse response = new UDTReturnReportDetailResponse();
    List<dynamic> oShipmentList = new List<dynamic>();
    List<UDTFileUploadError> errors = new List<UDTFileUploadError>();

    int i = 0;
    foreach (var data in excelDataList)
    {
      i++;
      UDTFileUploadError error = new UDTFileUploadError();

      #region Index 
      error.Row = i;
      error.IsSuccessed = true;
      #endregion

      #region CarrierTrackingNo
      if (!string.IsNullOrEmpty(data.OrderNo))
      {
        if (!string.IsNullOrEmpty(data.TrackingNo))
        {
          var order = orderList.FirstOrDefault(y => y.OrderNo == data.OrderNo && y.CarrierTrackingNo == data.TrackingNo);
          if (order is null)
          {
            error.IsSuccessed = false;
            error.Msg.Add($"Record not found against order no.: {data.OrderNo } and Tracking No.: {data.TrackingNo}");
          }
          else
          {
            if (order.CarrierId is not null)
            {
              if (order.CarrierId != carrierId)
              {
                error.IsSuccessed = false;
                error.Msg.Add($"The Order does not belong to the selected carrier, against order no.: {data.OrderNo } and Tracking No.: {data.TrackingNo}.");
              }
              else
              {
                oShipmentList.Add(order);
              }
            }
            else
            {
              error.IsSuccessed = false;
              error.Msg.Add($"The Order does not have a carrier against order no.: {data.OrderNo} and Tracking No.: {data.TrackingNo}.");
            }
          }
        }
        else
        {
          error.IsSuccessed = false;
          error.Msg.Add("TrackingNo is Missing.");
        }
      }
      else
      {
        error.IsSuccessed = false;
        error.Msg.Add("Order is missing.");
      }
      #endregion

      errors.Add(error);
      
    }
    response.IsSuccessed = errors.All(x => x.IsSuccessed);
    response.Data = oShipmentList;
    response.Errors = errors;
    return response;
  }
}
