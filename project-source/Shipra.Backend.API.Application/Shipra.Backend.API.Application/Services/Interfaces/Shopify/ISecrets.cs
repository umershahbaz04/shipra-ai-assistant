using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Services.Interfaces.Shopify;
public interface ISecrets
{
  string ShopifyClientSecret { get; }
  string ShopifyClientID { get; }
  string HostDomain { get; }
  string? AccessToken { get; }
  string ShopName { get; }
}
