using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IStationLookupRepository
{
  Task<List<ProductStation>?> GetAllStationLookup(ClientId clientId);
}
