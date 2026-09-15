using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.CommonAggregate;
public class Mcconfig
{
  public int Mcid { get; set; } 
  public string? Parameter { get; set; } 
  public string? Value { get; set; } 
  public string? Description { get; set; }
  public int? EnvironmentTypeId { get; set; }

}
