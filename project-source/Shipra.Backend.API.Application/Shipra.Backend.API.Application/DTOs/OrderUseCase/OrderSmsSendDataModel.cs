using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;
public class OrderSmsSendDataModel
{
  public string? RecipientMobile1 { get; set; }
  public string? DriverName { get; set; }
  public string? DriverMobile { get; set; }
  public Nullable<long> TrackingStatusID { get; set; }
  public string? Barcode { get; set; }
  public string? Recipient_Name { get; set; }
  public string? TrackingStatus { get; set; }
  public string? Tracking_no { get; set; }
  public string? TrackingStatus_Ar { get; set; }
  public string? Language { get; set; }
}
