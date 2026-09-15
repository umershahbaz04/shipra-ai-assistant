using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
public class KeyModel
{
  public string? PublicKey { get; set; }
  public string? UserName { get; set; }
  public string? Password { get; set; }
  public string? RandomKey { get; set; }
  public ClientId? ClientId { get; set; }
}
