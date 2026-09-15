using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.CatalougeAggregate;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IMasterDbRepository
{
  Task<Catalogue?> GetCatalogueByClientId(string clientId);
  Task<ShopifySession?> GetShopifySessionsByShop(string shop);
}
