using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IEmployeeRepository
{
  Task<Employee> CreateEmployee(Employee employee);
  Task<dynamic> GetAllEmployees(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, int? userRoleId, string clientId);
  Task<dynamic> GetEmployeeProfileById(string employeeId, string clientId);
  Task<Employee> GetEmployeeById(EmployeeId employeeId, ClientId clientId);
  Task<Employee> GetEmployeeByIdForEdit(EmployeeId employeeId, ClientId clientId);
  Task<dynamic> DeleteEmployee(Employee employee);
  Task<dynamic> UpdateEmployee(Employee employee);
  Task<Driver?> CheckEmployeeExists(EmployeeId? employeeId);
  Task<List<Employee>?> GetAllEmployeesForSelection(ClientId? clientId);
  Task<dynamic> GetEmployeesForShipperContract(string clientId);
  Task<string> GetEmployeeNextCode(ClientId clientId);
  Task<string> GetEmployeeNextCodeByUserName(string? userName,ClientId clientId, EmployeeId employeeId);
  Task<List<GenderLookup>> GetAllGenderForSelection();
  Task<Employee> GetEmployeeBySaleChannelConfigId(int saleChannelConfigId, ClientId? clientId);
  Task<List<Employee>> GetAllSalesPersonForSelection(ClientId? clientId);
  Task<List<EmployeeType>> GetAllEmployeeTypesAsync();
  Task<EmployeeAddress> GetEmployeeAddressById(EmployeeId? EmployeeId);
  Task<bool> CreateEmployeeAddress(EmployeeAddress oOrderAddress);
  Task<bool> UpdateEmployeeAddress(EmployeeAddress employeeAddress);
  Task<string?> GetEmployeeNameById(EmployeeId? employeeId);
  Task<int> GetEmployeeCountByClient(ClientId clientId);
  Task<List<Employee>> GetAllSaleConfigEmployeesByClient(ClientId clientId);
  Task<List<SaleChannelConfig>> GetAllSaleChannelConfigByEmployess(ClientId clientId);
  Task<bool> CreateEmployeeColumnConfiguration(EmployeeColumnConfiguration oEmployeeColumnConfiguration);
  Task<bool> UpdateEmployeeColumnConfiguration(EmployeeColumnConfiguration oEmployeeColumnConfiguration);
  Task<List<EmployeeColumnConfiguration>> GetAllEmployeeColumnConfiguration(ClientId? clientId);
}
