using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class S3ResponseModel
{
  public string? Message { get; set; }
  public string? Url { get; set; }
  public int Status { get; set; }
}
