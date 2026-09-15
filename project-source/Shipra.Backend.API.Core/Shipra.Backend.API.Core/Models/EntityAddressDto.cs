using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.CountryAggregate;

namespace Shipra.Backend.API.Core.Models;
public class EntityAddressDto
{
  public Country? Country { get; set; }
  public State? State { get; set; }
  public Province? Province { get; set; }
  public City? City { get; set; }
  public Area? Area { get; set; }
  public PinCode? PinCode { get; set; }
  public string? DisplayName { get; set; } 
}
