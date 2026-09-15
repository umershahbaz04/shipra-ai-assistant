using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.CatalougeAggregate;

namespace Shipra.Backend.API.Core.Models;
public class OperationStatusResponseModel
{
  public Catalogue? Catalogue { get; set; }
  public CatalogueDatabase? CatalogueDatabase { get; set; }
  public string? ConnectionString { get; set; } 
}
