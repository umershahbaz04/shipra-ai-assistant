using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ZokoReponseMessage
{
  public string? status { get; set; }
  public string? statusText { get; set; }
  public string? messageId { get; set; }
  public string? customerId { get; set; }
}
