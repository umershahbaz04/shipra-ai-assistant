using System;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Interfaces;

public interface IPerformanceReportRepository
{
  Task<dynamic> GetRoleForPerformanceReport(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, int roleId, string tenantId, string? countryIds = null);
  Task<dynamic> GetOrderByRole(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string EmployeeId, int roleName, string tenantId, string? countryIds = null);
}
