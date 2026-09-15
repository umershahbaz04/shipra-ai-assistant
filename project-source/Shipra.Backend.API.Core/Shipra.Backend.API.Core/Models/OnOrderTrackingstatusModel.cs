using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class OnOrderTrackingstatusModel
{
  public string? OrderNo { get; set; }
  public string? TrackingNo { get; set; }
  public string? Note { get; set; }
  public DateTime? OrderDate { get; set; }
  public string? Status { get; set; }
  public DateTime? StatusUpdatedOn { get; set; }
}
