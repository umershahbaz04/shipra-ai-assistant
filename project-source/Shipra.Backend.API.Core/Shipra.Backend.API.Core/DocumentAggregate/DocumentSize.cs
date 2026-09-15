using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.DocumentAggregate;
public class DocumentSize
{
  public int DocumentSizeId { get; set; } 
  public string? Name { get; set; } 
  public bool? Active { get; set; }
}
