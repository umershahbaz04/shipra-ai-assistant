using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ReturnAggregate;

namespace Shipra.Backend.API.Core.Models;
public class ReturnOrderWrapperResponseModel
{
  public Return? Return { get; set; }
  public bool? IsReturnExist { get; set; } = false;
  public bool? IsAllowReturn { get; set; } = true;
  public string? OrderId { get; set; }
  public string? ReturnId { get; set; }
  public int? OrderTypeId { get; set; } 
  public dynamic? OrderItems { get; set; }

}
