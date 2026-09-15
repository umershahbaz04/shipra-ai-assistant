using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IClientRepositoryInitializer
{
  Task<bool> CreatePrerequisiteClientData(string clientId);
  Task<Client?> GetLastClient(string clientId);
  Task<Client?> GetClientById(string clientId);
  Task<dynamic> CreateClient(Client client, ClientAddress clientAddress);
  Task<dynamic> UpdateClient(Client client); 
  Task<Employee> CreateEmployee(Employee employee);
  Task<string> GetClientNextStoreCode(ClientId? clientId); 
  Task<dynamic?> CreateStore(Store store);
  Task<ClientUserRole?> GetClientUserRoleByName(string salePerson, ClientId clientId);
  Task<string> GetEmployeeNextCode(ClientId clientId);
  Task<dynamic?> GetProductStationLookupByCityId(int? cityId); 
  Task<string> GetNextProductStationCode(ClientId clientId);
  Task<ProductStation> CreateProductStation(ProductStation model);
  Task<dynamic> CreateProductCategory(ProductCategory model);
  Task<List<Carrier>> GetAllCreatedClientCarrier(string clientId);
  Task<dynamic> CreateCarrier(Carrier model,string clientId);
  Task<dynamic> CreateActiveCarrier(ActiveCarrier model);
  Task<int> CreateExpenseCategoryForGeneralSetting(ClientId? clientId);
  Task<int> CreateDriverDefaultCTSSetting(ClientId? clientId, EmployeeId employeeId);
  Task<List<DefaultShipmentDashboard>?> GetDefaultShipmentDashboard();
  Task<bool> CreateShipmentGridClientSetting(ShipmentGridClientSetting oShipmentGridClientSetting);
  Task<bool> CreateShipmentGridColumn(ShipmentGridColumn oShipmentGridColumn);
  Task<List<CarrierTrackingStatusLookup>?> GetAllCarrierTrackingStatusLookup();
  Task<bool> CreateBatchClientCarrierTrackingStatus(List<ClientCarrierTrackingStatus> listOfClientCarrier);
  Task<bool> CreateCarrierFromLookup(string clientId);
  Task<bool> CreateEmployeeAddress(EmployeeAddress oOrderAddress,string? clientId);
  Task<bool> CreateStoreAddress(StoreAddress oStoreAddress, string clientIdStr);
  Task<ProductLinkToken> GetProductLinkTokenByToken(string? token);
  Task<dynamic> GetProductByIdForTakeOrder(string productId, int? storeId, string clientId);
}
