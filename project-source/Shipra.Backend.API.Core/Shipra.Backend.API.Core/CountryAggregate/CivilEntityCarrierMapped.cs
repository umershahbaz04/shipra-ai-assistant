using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.CountryAggregate;
//public class CivilEntityCarrierMapped
//{
//  public long CivilEntityCarrierMappedId { get; set; } 
//  public int? CivilEntityTypeId { get; set; } 
//  public int? MappedId { get; set; }  
//  public int? CarrierId { get; set; }
//  public int? DmsTypeId { get; set; }
//}
public class CivilEntityExtended
{
  public long CivilEntityExtendedId { get; set; }
  public int? CivilEntityTypeId { get; set; }
  public int? MappedId { get; set; }
  public string? CivilEntityName { get; set; }
  public string? ExtendedId { get; set; }
  public int? DmsTypeId { get; set; }
  public int? CarrierId { get; set; }
  public long? ParentId { get; set; }
}
