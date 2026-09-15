using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IProductStationRepository
{
  Task<ProductStation> CreateProductStation(ProductStation model);
  Task<dynamic> UpdateProductStation(ProductStation model);
  Task<ProductStation?> GetProductStationById(int productStationid);
  Task<ProductStation?> GetDefaultProductStation(ClientId clientId);
  Task<List<ProductStation>?> GetAllProductStations(ClientId clientId);
  Task<dynamic?> GetAllProductStations(int start,int length,string search,DateTime? createdFrom, DateTime? createdTo, string clientId);
  Task<dynamic?> GetProductStationLookupByRegionId(int? regionId);
  Task<bool> IsProductStationExist(string name, ClientId clientId);
  Task<string> GetNextProductStationCode(ClientId clientId);
}
