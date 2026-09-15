using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ControllerInfo
{
  public string ControllerName { get; set; } = string.Empty;
  public List<string> Actions { get; set; } = new();
}
