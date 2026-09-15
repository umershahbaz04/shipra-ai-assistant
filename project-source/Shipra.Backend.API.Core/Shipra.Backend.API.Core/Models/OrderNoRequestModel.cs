using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shipra.Backend.API.Core.Models;
 
public class OrderNoRequestModel
{
  public string? Request { get; set; }
  public string? Response { get; set; }
}
public class OrderNoModel
{
  public string? OrderNos { get; set; }

}


#region order Create Response
public class OrderCreateResponse
{
  public Result? Result { get; set; }
  public bool IsSuccess { get; set; }
  public Dictionary<string, string>? Errors { get; set; }
  public object? ConfigErrors { get; set; }
  public Dictionary<string, string>? ErrorCombined { get; set; }
  public int ErrorID { get; set; }
  public int StatusCode { get; set; }
}
public class Result
{
  public List<OrderData>? Data { get; set; }
  public string? Message { get; set; }
}
public class OrderData
{
  public string? OrderId { get; set; }
  public string? OrderNo { get; set; }
  public string? RefNo { get; set; }
  public bool IsSuccess { get; set; }
  public bool IsNewCreated { get; set; }
}
#endregion


#region Assigncarrier response 
public class AssignResult
{ 
  public string? tracking_no { get; set; }
  public string? orderNo { get; set; }
  public string? shipper_Ref { get; set; }
}
public class AssignCarrierResponse
{
  public List<AssignResult>? Result { get; set; }
  public bool IsSuccess { get; set; }
  public Dictionary<string, object>? Errors { get; set; }
  public object? ConfigErrors { get; set; }
  public Dictionary<string, object>? ErrorCombined { get; set; }
  public int ErrorID { get; set; }
  public int StatusCode { get; set; }
}
#endregion

#region Assign In House 
public class AssigninhouseResult
{
  public string? Data { get; set; }
  public string? Message { get; set; }
}
public class AssigninhouseResponse
{
  public AssigninhouseResult? Result { get; set; }
  public bool IsSuccess { get; set; }
  public object? Errors { get; set; }
  public object? ConfigErrors { get; set; }
  public object? ErrorCombined { get; set; }
  public int ErrorID { get; set; }
  public int StatusCode { get; set; }
}
#endregion
