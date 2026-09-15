using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IDriverRepository
{
  Task<Driver?> CreateDriver(Driver driver);
  Task<bool> DeleteDriver(Driver driver);
  Task<dynamic> GetAllDrivers(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId);
  Task<dynamic?> GetAllDriversForSelection(ClientId clientId);
  Task<Driver?> GetDriverById(DriverId driverId);
  Task<string> GetDriverNextCode(ClientId clientId);
  Task<Driver?> UpdateDriver(Driver driver);
  Task<Driver?> GetDriverByEmployeeId(EmployeeId employeeId);
  Task<dynamic> GetDriverProfile(string driverId,string clientId);
  #region carrier status
  Task<bool> DeleteDriverCTSSetting(DriverCTSSettingId? driverCtsid);  
  Task<bool> DeleteDriverCTSSetting(ClientId clientId);
  Task<dynamic> GetAllDriverCTSSetting(string clientId);
  Task<DriverCTSSetting> CreateDriverCTSSetting(DriverCTSSetting driverCtssetting);
  Task<int> CreateDriverDefaultCTSSetting(ClientId? clientId, EmployeeId employeeId);
  #endregion

}
