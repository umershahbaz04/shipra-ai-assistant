using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;

public class PerformanceReportRepository : IPerformanceReportRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _dbContext;

  public PerformanceReportRepository(DapperAppDbContext dapperAppDbContext, AppDbContext dbContext)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _dbContext = dbContext;
  }

  public async Task<dynamic> GetRoleForPerformanceReport(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, int roleName, string tenantId, string? countryIds = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(tenantId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(tenantId, _dbContext);
      var dynamicParams = new DynamicParameters();

      dynamicParams.Add("@TenantId", tenantId);
      dynamicParams.Add("@displayStart", start);
      dynamicParams.Add("@displayLength", length);

      var columnsSql = @"
             SELECT 
                 c.ColumnName, 
                 s.DashboardStatusValue 
             FROM dbo.ShipmentGridColumn c
             JOIN dbo.ShipmentGridClientSetting s ON c.ShipmentGridColumnId = s.ShipmentGridColumnId
             WHERE c.ClientId = @TenantId AND c.Active = 1 AND s.Active = 1
             ORDER BY c.DisplayOrder";
         
      var columnsData = (await connection.QueryAsync<dynamic>(columnsSql, new { TenantId = tenantId })).ToList();

      string dynamicColumns = "";
      foreach (var col in columnsData)
      {
          string colName = col.ColumnName;
          if (string.Equals(colName, "All", StringComparison.OrdinalIgnoreCase))
              continue;

          string propertyName = colName.Replace(" ", "_");
          string dashboardStatusValue = col.DashboardStatusValue;
          
          if (!string.IsNullOrWhiteSpace(dashboardStatusValue) && dashboardStatusValue != "0")
          {
              dynamicColumns += $",\n                COUNT(DISTINCT CASE WHEN o.CarrierTrackingStatusId IN ({dashboardStatusValue}) THEN o.OrderId ELSE NULL END) AS [{propertyName}]";
          }
      }

      string join = roleName == 1
          ? @"
                INNER JOIN DeliveryNoteDetail dnd
                    ON o.OrderId = dnd.OrderId
                    AND dnd.Active = 1
                INNER JOIN DeliveryNote dn
                    ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                    AND dn.Active = 1
                INNER JOIN Driver d
                    ON dn.DriverId = d.DriverId
                    AND d.Active = 1
                INNER JOIN Employee e
                    ON d.EmployeeId = e.EmployeeId"
          : @"
                INNER JOIN Employee e
                    ON o.SaleChannelConfigId = e.SaleChannelConfigId";

      string leadsCountSelect = roleName == 2
          ? $@",
                (SELECT COUNT(1) 
                 FROM dbo.Leads l 
                 WHERE l.SalespersonId = e.EmployeeId 
                   AND (l.Active = 1 OR l.Active IS NULL) 
                   AND l.ClientId = @TenantId"
                 + (createdFrom != null ? $" AND CAST({CommonUtility.GetFormatedDateStr("l.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)" : "")
                 + (createdTo != null ? $" AND CAST({CommonUtility.GetFormatedDateStr("l.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)" : "")
                 + (!string.IsNullOrWhiteSpace(countryIds) ? " AND l.CountryId IN (SELECT value FROM STRING_SPLIT(@countryIds, ','))" : "")
                 + ") AS TotalLeads"
          : "";

      string query = $@"
            SELECT
                ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS RowNum,
                COUNT(*) OVER() AS TotalCount,
                e.EmployeeId AS Id,
                e.EmployeeName AS Name,
                COUNT(DISTINCT o.OrderId) AS OrderCount{leadsCountSelect}{dynamicColumns}
            FROM dbo.[Order] o
            INNER JOIN dbo.OrderAddress oa
                ON oa.OrderAddressId = o.OrderAddressId
            {join}";

      string where = " WHERE 1 = 1 ";

      where += " AND o.ClientId = @TenantId ";
      where += " AND e.Active = 1 ";

      if (!string.IsNullOrWhiteSpace(countryIds))
      {
        dynamicParams.Add("@countryIds", countryIds);
        where += " AND (oa.CountryId IN (SELECT value FROM STRING_SPLIT(@countryIds, ','))) ";
      }

      if (!string.IsNullOrWhiteSpace(search))
      {
        dynamicParams.Add("@Search", "%" + search + "%");
        where += " AND e.EmployeeName LIKE @Search ";
      }

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        where += $" AND CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE) ";
      }

      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        where += $" AND CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE) ";
      }

      string groupBy = @"
            GROUP BY
                e.EmployeeId,
                e.EmployeeName";

      Dictionary<int, string> sortColumns = new()
        {
            {0, "Name"},
            {1, "OrderCount"}
        };

      string orderBy = sortColumns.ContainsKey(sortCol)
          ? sortColumns[sortCol]
          : "Name";

      string sql = query
                 + where
                 + groupBy
                 + $" ORDER BY {orderBy} {(string.IsNullOrWhiteSpace(sortDir) ? "DESC" : sortDir)} "
                 + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY;";

      var data = (await connection.QueryAsync(sql, dynamicParams)).ToList();

      dynamic result = new ExpandoObject();
      result.TotalCount = data.Any() ? data.First().TotalCount : 0;
      result.List = data;

      return result;
    }
  }
  public async Task<dynamic> GetOrderByRole(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string employeeId, int roleName, string tenantId, string? countryIds = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(tenantId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(tenantId, _dbContext);

      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@Id", employeeId);
      dynamicParams.Add("@TenantId", tenantId);
      dynamicParams.Add("@displayStart", start);
      dynamicParams.Add("@displayLength", length);

      string join = roleName == 1
                        ? @"
                              INNER JOIN DeliveryNoteDetail dnd
                                  ON o.OrderId = dnd.OrderId
                                  AND dnd.Active = 1
                              INNER JOIN DeliveryNote dn
                                  ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                                  AND dn.Active = 1
                              INNER JOIN Driver d
                                  ON dn.DriverId = d.DriverId
                              INNER JOIN Employee e
                                  ON d.EmployeeId = e.EmployeeId"
                        : @"
                              INNER JOIN Employee e
                                  ON o.SaleChannelConfigId = e.SaleChannelConfigId";

      string query = $@"
                    SELECT
                        ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS RowNum,
                        COUNT(*) OVER() AS TotalCount,
                        o.OrderId,
                        o.OrderNo,
                        o.OrderDate,
                        o.CreatedOn,
                        o.Amount,
                        o.CarrierTrackingNo,
                        o.CarrierTrackingStatus,
                        o.CarrierTrackingStatusId,
                        oa.CustomerFullAddress,
                        oa.Email,
                        oa.Mobile1,
                        oa.Mobile2,
                        e.EmployeeId,
                        e.EmployeeName
                    FROM dbo.[Order] o
                    INNER JOIN dbo.OrderAddress oa
                        ON oa.OrderAddressId = o.OrderAddressId
                    {join}";

      string where = " WHERE 1 = 1 ";

      where += " AND o.ClientId = @TenantId ";

      if (!string.IsNullOrWhiteSpace(countryIds))
      {
        dynamicParams.Add("@countryIds", countryIds);
        where += " AND (oa.CountryId IN (SELECT value FROM STRING_SPLIT(@countryIds, ','))) ";
      }

      if (roleName == 1)
        where += " AND d.EmployeeId = @Id ";
      else
        where += " AND e.EmployeeId = @Id ";

      if (!string.IsNullOrWhiteSpace(search))
      {
        dynamicParams.Add("@Search", "%" + search + "%");
        where += " AND o.OrderNo LIKE @Search ";
      }

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        where += $" AND CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE) ";
      }

      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        where += $" AND CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE) ";
      }

      Dictionary<int, string> sortColumns = new()
        {
            {0, "o.OrderNo"},
            {1, "o.OrderDate"},
            {2, "o.Amount"},
            {3, "o.CreatedOn"}
        };

      string orderBy = sortColumns.ContainsKey(sortCol)
          ? sortColumns[sortCol]
          : "o.CreatedOn";

      string sql = query
                 + where
                 + $" ORDER BY {orderBy} {(string.IsNullOrWhiteSpace(sortDir) ? "DESC" : sortDir)} "
                 + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY;";

      var data = (await connection.QueryAsync(sql, dynamicParams)).ToList();

      dynamic result = new ExpandoObject();
      result.TotalCount = data.Any() ? data.First().TotalCount : 0;
      result.List = data;

      return result;
    }
  }
}
