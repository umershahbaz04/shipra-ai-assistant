using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.LeadAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;

public class LeadRepository : ILeadRepository
{
    private readonly DapperAppDbContext _dapperAppDbContext;
    private readonly AppDbContext _context;

    public LeadRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
    {
        _dapperAppDbContext = dapperAppDbContext;
        _context = context;
    }

    public async Task<Lead?> CheckLeadExistsByPhoneOrProduct(ClientId clientId, string phoneNumber, string productName)
    {
        return await _context.Leads.FirstOrDefaultAsync(x => x.ClientId! == clientId && (x.PhoneNumber == phoneNumber && x.ProductName == productName));
    }
    public async Task<string?> GetNonCompletedLeadStatusByPhone(ClientId clientId, string phoneNumber)
    {
        var completedStatusIds = await _context.ClientLeadStatusLookups
            .Where(x => x.ClientId == clientId && x.Active == true && x.Description != null && x.Description.ToLower() == "completed")
            .Select(x => x.ClientLeadStatusId)
            .ToListAsync();

        var existingLead = await _context.Leads
            .FirstOrDefaultAsync(x => x.ClientId == clientId 
                && x.Active == true 
                && x.PhoneNumber == phoneNumber 
                && (x.LeadStatusId == null || !completedStatusIds.Contains(x.LeadStatusId.Value)));

        if (existingLead != null)
        {
            if (existingLead.LeadStatusId == null) return "Unknown";
            var status = await _context.ClientLeadStatusLookups
                .FirstOrDefaultAsync(x => x.ClientLeadStatusId == existingLead.LeadStatusId.Value);
            return status?.Description ?? "Unknown";
        }

        return null;
    }

    public async Task<Lead> CreateLead(Lead lead)
    {
        await _context.Leads.AddAsync(lead);
        await _context.SaveChangesAsync();
        return lead;
    }

    public async Task<Lead?> UpdateLead(Lead lead)
    {
        _context.Leads.Update(lead);
        await _context.SaveChangesAsync();
        return lead;
    }

    public async Task<Lead?> GetLeadById(LeadId leadId)
    {
        return await _context.Leads.FirstOrDefaultAsync(x => x.LeadId == leadId);
    }

    public async Task<bool> DeleteLeads(List<Lead> leads)
    {
        _context.Leads.RemoveRange(leads);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<LeadStatusLookup>> GetAllLeadStatusForSelection()
    {
        return await _context.LeadStatusLookups.Where(x => x.Active == true).ToListAsync();
    }

    public async Task<dynamic> GetAllLeads(string clientId, int start, int length, string? search = null, int sortCol = 0, string? sortDir = null, string? leadStatusIds = null, string? salespersonIds = null, string? assignmentFilter = null, string? countryIds = null, string? startDate = null, string? endDate = null)
    {
        using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
        {
            var dynamicParams = new DynamicParameters();

            string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY l.CreatedOn DESC) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               l.LeadId,
                               ISNULL(od.OrderNo,'') AS DraftOrderNo,
                               ISNULL(o.OrderNo,'') AS OrderNo,
                               CASE
                                   WHEN l.OrderDraftId IS NOT NULL THEN jd.CustomerName
                                   ELSE oa.CustomerName
                               END AS CustomerName,
                               CASE
                                   WHEN l.OrderDraftId IS NOT NULL THEN jd.CustomerFullAddress
                                   ELSE oa.CustomerFullAddress
                               END AS CustomerFullAddress,
                               CASE
                                   WHEN l.OrderDraftId IS NOT NULL THEN jd.Amount
                                   ELSE o.Amount
                               END AS Amount,
                               l.PhoneNumber,
                               l.ProductName,
                               l.GoogleLocationLink,
                               l.SalespersonId,
                               ISNULL(e.EmployeeName, '') AS SalespersonName,
                               l.LeadStatusId,
                               ISNULL(clsl.Description, '') AS LeadStatus,
                               ISNULL(c.Name, '') AS CountryName,
                               l.Active,
                               l.CreatedOn,
                               l.OrderDraftId,
                               l.OrderId
                        FROM dbo.Leads l
                            LEFT JOIN dbo.Employee e ON e.EmployeeId = l.SalespersonId
                            LEFT JOIN dbo.ClientLeadStatusLookup clsl ON clsl.ClientLeadStatusId = l.LeadStatusId
                            LEFT JOIN dbo.Country c ON c.CountryId = l.CountryId
                            LEFT JOIN dbo.OrderDraft od ON od.OrderDraftId = l.OrderDraftId
                            OUTER APPLY
                            (
                                SELECT *
                                FROM OPENJSON(od.OrderInfo)
                                WITH
                                (
                                    OrderNo NVARCHAR(100) '$.OrderNo',
                                    CustomerName NVARCHAR(250) '$.OrderAddress.CustomerName',
                                    CustomerFullAddress NVARCHAR(1000) '$.OrderAddress.CustomerFullAddress',
                                    Amount DECIMAL(18, 2) '$.Amount',
                                    CountryId INT '$.OrderAddress.CountryId',
                                    CityId INT '$.OrderAddress.CityId',
                                    StateId INT '$.OrderAddress.StateId',
                                    ProvinceId INT '$.OrderAddress.ProvinceId',
                                    AreaId INT '$.OrderAddress.AreaId',
                                    PinCodeId INT '$.OrderAddress.PinCodeId'
                                )
                            ) jd
                            LEFT JOIN dbo.[Order] o ON o.OrderId = l.OrderId
                            LEFT JOIN dbo.OrderAddress oa ON oa.OrderAddressId = o.OrderAddressId ";

            string whereStart = $"WHERE ( l.Active = 1 AND l.ClientId = '{clientId}' ";
            string whereEnd = ")";

            dynamicParams.Add("displayStart", start);
            dynamicParams.Add("displayLength", length);

            if (!string.IsNullOrEmpty(search))
            {
                var firstToken = search.Split(',').FirstOrDefault();
                if (Guid.TryParse(firstToken, out _))
                {
                    whereStart += $"AND (l.LeadId IN (SELECT CAST(value AS UNIQUEIDENTIFIER) FROM STRING_SPLIT('{search}', ','))) ";
                }
                else
                {
                    dynamicParams.Add("@search", $"%{search}%");
                    whereStart += "AND (l.PhoneNumber LIKE @search OR l.ProductName LIKE @search) ";
                }
            }
            if (!string.IsNullOrEmpty(leadStatusIds))
            {
                whereStart += $"AND (l.LeadStatusId IN (SELECT value FROM STRING_SPLIT('{leadStatusIds}', ','))) ";
            }
            if (!string.IsNullOrEmpty(salespersonIds))
            {
                whereStart += $"AND (l.SalespersonId IN (SELECT value FROM STRING_SPLIT('{salespersonIds}', ','))) ";
            }

            if (!string.IsNullOrEmpty(assignmentFilter))
            {
                if (assignmentFilter.Equals("assigned", StringComparison.OrdinalIgnoreCase))
                {
                    whereStart += "AND (l.SalespersonId IS NOT NULL) ";
                }
                else if (assignmentFilter.Equals("unassigned", StringComparison.OrdinalIgnoreCase))
                {
                    whereStart += "AND (l.SalespersonId IS NULL) ";
                }
            }

            if (!string.IsNullOrEmpty(countryIds))
            {
                dynamicParams.Add("@CountryIds", countryIds);
                whereStart += " AND l.CountryId IN (SELECT value FROM STRING_SPLIT(@CountryIds, ',')) ";
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                dynamicParams.Add("@StartDate", startDate);
                dynamicParams.Add("@EndDate", endDate);
                whereStart += " AND CAST(l.CreatedOn as Date) >= @StartDate AND CAST(l.CreatedOn as Date) <= @EndDate ";
            }



            string sortColName = "l.CreatedOn";
            if (sortCol == 1) sortColName = "l.ProductName";
            else if (sortCol == 2) sortColName = "l.PhoneNumber";

            if (string.IsNullOrEmpty(sortDir)) sortDir = "DESC";

            string completeQuery = $@"{query} 
                                      {whereStart} {whereEnd} 
                                      ORDER BY {sortColName} {sortDir} 
                                      OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY";

            var result = await connection.QueryAsync<dynamic>(completeQuery, dynamicParams);
            return result.ToList();
        }
    }

    // ──────────────────────────────────────────────────────────────
    // Lead Tabs Config — mirrors GetAllShipmentGridClientSettingForDashboard
    // ──────────────────────────────────────────────────────────────
    public async Task<List<LeadDashboardResponseModel>> GetAllLeadGridClientSettingForDashboard(string? clientId)
    {
        using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("@ClientId", clientId);

            string query = $@"
                SELECT 
                    lgc.LeadGridColumnId,
                    lgc.ColumnName AS DashboardStatusName,
                    REPLACE(lgc.ColumnName, ' ', '') AS DashboardStatusNameForKey,
                    lgc.DisplayOrder,
                    lgc.IsDisplay,
                    ISNULL(lgc.Active, 0) AS Active,
                    ISNULL(lgc.IsDefaultStatusTab, 1) AS IsDefaultStatusTab,
                    lgcs.DashboardStatusValue
                FROM dbo.LeadGridColumn AS lgc
                    INNER JOIN dbo.LeadGridClientSetting AS lgcs
                        ON lgcs.LeadGridColumnId = lgc.LeadGridColumnId
                WHERE lgc.ClientId = @ClientId

                ORDER BY lgc.DisplayOrder
            ";

            var data = await connection.QueryAsync<LeadDashboardResponseModel>(query, dynamicParams);
            return data.ToList();
        }
    }

    // ──────────────────────────────────────────────────────────────
    // Lead Tabs Count — mirrors GetAllShipmentTabsCount dynamic pivot
    // ──────────────────────────────────────────────────────────────
    public async Task<dynamic> GetAllLeadTabsCount(string clientId, string? leadStatusIds = null, string? salespersonIds = null, string? search = null, string? assignmentFilter = null, string? countryIds = null, string? startDate = null, string? endDate = null)
    {
        using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
        {
            var dynamicParams = new DynamicParameters();

            // Get tab config for this client
            List<LeadDashboardResponseModel> tabConfig = await GetAllLeadGridClientSettingForDashboard(clientId);

            if (tabConfig == null || tabConfig.Count == 0)
                return new List<dynamic>();

            // Build dynamic SELECT columns (same approach as GetAllShipmentTabsCount2134)
            var sb = new StringBuilder();
            for (int i = 0; i < tabConfig.Count; i++)
            {
                string comma = i < tabConfig.Count - 1 ? ", " : "";
                var item = tabConfig[i];
                // Use IsDefaultStatusTab (not name string) so renaming the tab never breaks the count
                if (item.IsDefaultStatusTab)
                {
                    sb.Append($"TotalLeads = COUNT(*){comma} ");
                }
                else
                {
                    // If DashboardStatusValue is null/empty the tab has no statuses mapped → count 0
                    string statusVal = string.IsNullOrEmpty(item.DashboardStatusValue) ? "-1" : item.DashboardStatusValue;
                    sb.Append($"[{item.DashboardStatusNameForKey}] = COUNT(CASE WHEN l.LeadStatusId IN ({statusVal}) THEN 0 END){comma} ");
                }
            }

            string whereClause = $"WHERE l.Active = 1 AND l.ClientId = '{clientId}' ";

            if (!string.IsNullOrEmpty(search))
            {
                dynamicParams.Add("@search", $"%{search}%");
                whereClause += "AND (l.PhoneNumber LIKE @search OR l.ProductName LIKE @search) ";
            }
            if (!string.IsNullOrEmpty(salespersonIds))
            {
                whereClause += $"AND (l.SalespersonId IN (SELECT value FROM STRING_SPLIT('{salespersonIds}', ','))) ";
            }

            if (!string.IsNullOrEmpty(assignmentFilter))
            {
                if (assignmentFilter.Equals("assigned", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause += "AND (l.SalespersonId IS NOT NULL) ";
                }
                else if (assignmentFilter.Equals("unassigned", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause += "AND (l.SalespersonId IS NULL) ";
                }
            }

            string joinClause = "";

            if (!string.IsNullOrEmpty(countryIds))
            {
                dynamicParams.Add("@CountryIds", countryIds);
                whereClause += " AND l.CountryId IN (SELECT value FROM STRING_SPLIT(@CountryIds, ',')) ";
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                dynamicParams.Add("@StartDate", startDate);
                dynamicParams.Add("@EndDate", endDate);
                whereClause += " AND CAST(l.CreatedOn as Date) >= @StartDate AND CAST(l.CreatedOn as Date) <= @EndDate ";
            }



            string query = $@"
                SELECT {sb}
                FROM dbo.Leads l
                {joinClause}
                {whereClause}
            ";

            var result = await connection.QueryAsync<dynamic>(query, dynamicParams);
            return result.ToList();
        }
    }

    public async Task<dynamic> GetSalesPersonLeadStats(string clientId, string? leadStatusIds = null, string? salespersonIds = null, string? search = null, string? assignmentFilter = null, string? countryIds = null, string? startDate = null, string? endDate = null)
    {
        using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
        {
            var dynamicParams = new DynamicParameters();

            List<LeadDashboardResponseModel> tabConfig = await GetAllLeadGridClientSettingForDashboard(clientId);

            if (tabConfig == null || tabConfig.Count == 0)
                return new List<dynamic>();

            var sb = new StringBuilder();
            for (int i = 0; i < tabConfig.Count; i++)
            {
                string comma = i < tabConfig.Count - 1 ? ", " : "";
                var item = tabConfig[i];
                if (item.IsDefaultStatusTab)
                {
                    sb.Append($"TotalCount = COUNT(*){comma} ");
                }
                else
                {
                    string statusVal = string.IsNullOrEmpty(item.DashboardStatusValue) ? "-1" : item.DashboardStatusValue;
                    sb.Append($"[{item.DashboardStatusNameForKey}] = COUNT(CASE WHEN l.LeadStatusId IN ({statusVal}) THEN 0 END){comma} ");
                }
            }

            string whereClause = $"WHERE l.Active = 1 AND l.ClientId = '{clientId}' ";

            if (!string.IsNullOrEmpty(search))
            {
                dynamicParams.Add("@search", $"%{search}%");
                whereClause += "AND (l.PhoneNumber LIKE @search OR l.ProductName LIKE @search) ";
            }
            if (!string.IsNullOrEmpty(salespersonIds))
            {
                whereClause += $"AND (l.SalespersonId IN (SELECT value FROM STRING_SPLIT('{salespersonIds}', ','))) ";
            }

            if (!string.IsNullOrEmpty(assignmentFilter))
            {
                if (assignmentFilter.Equals("assigned", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause += "AND (l.SalespersonId IS NOT NULL) ";
                }
                else if (assignmentFilter.Equals("unassigned", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause += "AND (l.SalespersonId IS NULL) ";
                }
            }

            string joinClause = "LEFT JOIN dbo.Employee sp ON sp.EmployeeId = l.SalespersonId";

            if (!string.IsNullOrEmpty(countryIds))
            {
                dynamicParams.Add("@CountryIds", countryIds);
                whereClause += " AND l.CountryId IN (SELECT value FROM STRING_SPLIT(@CountryIds, ',')) ";
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                dynamicParams.Add("@StartDate", startDate);
                dynamicParams.Add("@EndDate", endDate);
                whereClause += " AND CAST(l.CreatedOn as Date) >= @StartDate AND CAST(l.CreatedOn as Date) <= @EndDate ";
            }

            string query = $@"
                SELECT 
                    ISNULL(sp.EmployeeName, 'Unassigned') AS Name,
                    l.SalespersonId AS Id,
                    {sb}
                FROM dbo.Leads l
                {joinClause}
                {whereClause}
                GROUP BY l.SalespersonId, sp.EmployeeName
            ";

            var result = await connection.QueryAsync<dynamic>(query, dynamicParams);
            return result.ToList();
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Lead Tab Management (mirrors CreateShipmentGridColumn / UpdateShipmentTabDisplayOrder)
    // ─────────────────────────────────────────────────────────────────
    public async Task<bool> CreateLeadGridColumn(string columnName, string? dashboardStatusIdValues, ClientId clientId, EmployeeId employeeId)
    {
        // Get next display order
        int displayOrder = (_context.LeadGridColumns
            .Where(x => x.ClientId == clientId && x.Active == true)
            .OrderByDescending(x => x.DisplayOrder)
            .FirstOrDefault()?.DisplayOrder ?? 0) + 1;

        var column = LeadGridColumn.Create(columnName, displayOrder, null, clientId, employeeId);
        _context.LeadGridColumns.Add(column);
        await _context.SaveChangesAsync();

        var setting = LeadGridClientSetting.Create(column.LeadGridColumnId, dashboardStatusIdValues, clientId, employeeId);
        _context.LeadGridClientSettings.Add(setting);
        await _context.SaveChangesAsync();

        // Remove these statuses from any other existing tabs for this client
        if (!string.IsNullOrEmpty(dashboardStatusIdValues))
        {
            var newStatusIds = dashboardStatusIdValues.Split(',').Select(x => x.Trim()).ToList();
            var existingSettings = await _context.LeadGridClientSettings
                .Where(x => x.ClientId == clientId && x.LeadGridColumnId != column.LeadGridColumnId && x.Active == true)
                .ToListAsync();

            foreach (var ext in existingSettings)
            {
                if (!string.IsNullOrEmpty(ext.DashboardStatusValue))
                {
                    var extStatusIds = ext.DashboardStatusValue.Split(',').Select(x => x.Trim()).ToList();
                    var updatedStatusIds = extStatusIds.Where(x => !newStatusIds.Contains(x)).ToList();
                    string newValue = string.Join(",", updatedStatusIds);
                    
                    ext.UpdateDashboardStatusValue(newValue, employeeId);
                    _context.LeadGridClientSettings.Update(ext);
                }
            }
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> UpdateLeadTabDisplayOrder(List<(int leadGridColumnId, int displayOrder)> items)
    {
        foreach (var (id, order) in items)
        {
            var col = await _context.LeadGridColumns.FindAsync(id);
            if (col != null)
            {
                col.DisplayOrder = order;
            }
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<dynamic> GetLeadContacts(string clientId, int start, int length, string? search)
    {
        using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("displayStart", start);
            dynamicParams.Add("displayLength", length);
            dynamicParams.Add("clientId", clientId);

            string query = $@"
                WITH ContactCTE AS (
                    SELECT 
                        oa.Mobile1 AS Mobile,
                        MAX(oa.CustomerName) AS CustomerName,
                        COUNT(o.OrderId) AS TotalOrders,
                        MAX(o.CreatedOn) AS LastOrderDate
                    FROM [dbo].[OrderAddress] AS oa
                    INNER JOIN [dbo].[Order] AS o ON oa.OrderAddressId = o.OrderAddressId
                    WHERE o.ClientId = @clientId
                      AND oa.Mobile1 IS NOT NULL AND oa.Mobile1 <> ''
            ";

            if (!string.IsNullOrEmpty(search))
            {
                dynamicParams.Add("@search", $"%{search}%");
                query += " AND (oa.Mobile1 LIKE @search OR oa.CustomerName LIKE @search) ";
            }

            query += @"
                    GROUP BY oa.Mobile1
                )
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY LastOrderDate DESC) AS RowNum,
                    COUNT(*) OVER () AS TotalCount,
                    Mobile,
                    CustomerName,
                    TotalOrders
                FROM ContactCTE
                ORDER BY LastOrderDate DESC
                OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY
            ";

            var result = await connection.QueryAsync<dynamic>(query, dynamicParams);
            return result.ToList();
        }
    }

    public async Task<dynamic> GetOrdersByContactMobile(string clientId, string mobileNumber)
    {
        using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("clientId", clientId);
            dynamicParams.Add("mobileNumber", mobileNumber);

            string query = $@"
                SELECT 
                    o.OrderNo,
                    oa.CustomerName,
                    oa.CustomerFullAddress,
                    o.OrderDate,
                    o.Description
                FROM [dbo].[Order] AS o
                INNER JOIN [dbo].[OrderAddress] AS oa ON o.OrderAddressId = oa.OrderAddressId
                WHERE o.ClientId = @clientId
                  AND oa.Mobile1 = @mobileNumber
                ORDER BY o.OrderDate DESC
            ";

            var result = await connection.QueryAsync<dynamic>(query, dynamicParams);
            return result.ToList();
        }
    }

    // ──────────────────────────────────────────────────────────────
    // Client Lead Status CRUD
    // ──────────────────────────────────────────────────────────────

    public async Task<bool> CreateClientLeadStatus(ClientLeadStatusLookup clientLeadStatus)
    {
        await _context.ClientLeadStatusLookups.AddAsync(clientLeadStatus);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ClientLeadStatusLookup?> GetClientLeadStatusById(int clientLeadStatusId, ClientId clientId)
    {
        return await _context.ClientLeadStatusLookups
            .FirstOrDefaultAsync(x => x.ClientLeadStatusId == clientLeadStatusId && x.ClientId == clientId);
    }

    public async Task<bool> UpdateClientLeadStatus(ClientLeadStatusLookup clientLeadStatus)
    {
        _context.ClientLeadStatusLookups.Update(clientLeadStatus);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ClientLeadStatusLookup>> GetAllClientLeadStatusForSelection(ClientId clientId)
    {
        return await _context.ClientLeadStatusLookups
            .Where(x => x.ClientId == clientId && x.Active == true)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<dynamic> GetAllClientLeadStatus(string clientId, int start, int length, string? search, int sortCol, string? sortDir)
    {
        using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("displayStart", start);
            dynamicParams.Add("displayLength", length);

            string whereClause = $"WHERE cls.ClientId = '{clientId}' ";

            if (!string.IsNullOrEmpty(search))
            {
                dynamicParams.Add("@search", $"%{search}%");
                whereClause += "AND cls.Description LIKE @search ";
            }

            string sortColName = "cls.DisplayOrder";
            if (sortCol == 1) sortColName = "cls.Description";
            if (string.IsNullOrEmpty(sortDir)) sortDir = "ASC";

            string query = $@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY {sortColName} {sortDir}) AS RowNum,
                    COUNT(*) OVER () AS TotalCount,
                    cls.ClientLeadStatusId,
                    cls.Description,
                    cls.DisplayOrder,
                    cls.Active,
                    cls.CreatedOn
                FROM dbo.ClientLeadStatusLookup cls
                {whereClause}
                ORDER BY {sortColName} {sortDir}
                OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY
            ";

            var result = await connection.QueryAsync<dynamic>(query, dynamicParams);
            return result.ToList();
        }
    }

    public async Task<int> GetDefaultClientLeadStatusId(ClientId clientId)
    {
        // Get the very first status by DisplayOrder (this represents the initial state, e.g., "Open")
        // This avoids hardcoding strings like "Open" so the user can rename their statuses freely.
        var openStatus = await _context.ClientLeadStatusLookups
            .Where(x => x.ClientId == clientId && x.Active == true)
            .OrderBy(x => x.DisplayOrder)
            .FirstOrDefaultAsync();
        
        // Return 1 (the global ID) as fallback, but typically this will return 7, 8, etc.
        return openStatus?.ClientLeadStatusId ?? 1;
    }

    /// <summary>
    /// Gets a single LeadGridColumn by ID for the given client.
    /// </summary>
    public async Task<LeadGridColumn?> GetLeadGridColumnById(int leadGridColumnId, ClientId clientId)
    {
        return await _context.LeadGridColumns
            .FirstOrDefaultAsync(x => x.LeadGridColumnId == leadGridColumnId && x.ClientId == clientId && x.Active == true);
    }

    /// <summary>
    /// Updates the column name and DashboardStatusValue for a tab (mirrors UpdateShipmentGridColumn).
    /// </summary>
    public async Task<bool> UpdateLeadGridColumn(int leadGridColumnId, string columnName, string dashboardStatusValue, ClientId clientId, EmployeeId employeeId)
    {
        var col = await GetLeadGridColumnById(leadGridColumnId, clientId);
        if (col is null) return false;

        col.ColumnName = columnName;
        col.UpdatedBy = employeeId;
        col.UpdatedOn = DateTime.UtcNow;
        _context.LeadGridColumns.Update(col);

        // Find setting using ONLY the integer FK (LeadGridColumnId).
        // Avoid comparing ClientId value objects — EF Core cannot reliably
        // translate record equality into SQL, causing the query to return null
        // and silently skipping the DashboardStatusValue update.
        var setting = await _context.LeadGridClientSettings
            .FirstOrDefaultAsync(x => x.LeadGridColumnId == leadGridColumnId);
        if (setting is not null)
        {
            setting.UpdateDashboardStatusValue(dashboardStatusValue, employeeId);
            _context.LeadGridClientSettings.Update(setting);
        }
        else
        {
            // Setting row is missing — create it so the value is never lost
            var newSetting = LeadGridClientSetting.Create(leadGridColumnId, dashboardStatusValue, clientId, employeeId);
            _context.LeadGridClientSettings.Add(newSetting);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Soft-deletes a LeadGridColumn and its associated LeadGridClientSetting.
    /// </summary>
    public async Task<bool> DeleteLeadGridColumn(LeadGridColumn column)
    {
        // Remove linked setting
        var setting = await _context.LeadGridClientSettings
            .FirstOrDefaultAsync(x => x.LeadGridColumnId == column.LeadGridColumnId && x.ClientId == column.ClientId);
        if (setting is not null)
            _context.LeadGridClientSettings.Remove(setting);

        // Hard-delete the column
        _context.LeadGridColumns.Remove(column);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Comprehensive seeding for any new client.
    /// Ensures all 3 Lead tables are populated:
    ///   1. LeadStatusLookup  (global system defaults — seeds if empty)
    ///   2. LeadGridColumn + LeadGridClientSetting  (per-client tabs)
    ///   3. ClientLeadStatusLookup  (per-client status list)
    /// All steps are idempotent — safe to call on every request.
    /// </summary>
    public async Task SeedDefaultLeadStatuses(ClientId clientId, EmployeeId employeeId)
    {
        // ── Step 1: Ensure LeadStatusLookup (global) has default data ──────
        bool hasGlobalStatuses = await _context.LeadStatusLookups.AnyAsync();
        if (!hasGlobalStatuses)
        {
            // ValueGeneratedNever — must supply explicit IDs
            var defaultLookups = new List<LeadStatusLookup>
            {
                new LeadStatusLookup { LeadStatusId = 1, Description = "Open",        Active = true },
                new LeadStatusLookup { LeadStatusId = 2, Description = "In Progress", Active = true },
                new LeadStatusLookup { LeadStatusId = 3, Description = "Called",      Active = true },
                new LeadStatusLookup { LeadStatusId = 4, Description = "Completed",   Active = true },
                new LeadStatusLookup { LeadStatusId = 5, Description = "Cancelled",   Active = true },
            };
            _context.LeadStatusLookups.AddRange(defaultLookups);
            await _context.SaveChangesAsync();
        }

        // Load global statuses (used for both tab mapping and client status list)
        var globalStatuses = await _context.LeadStatusLookups
            .Where(x => x.Active == true)
            .OrderBy(x => x.LeadStatusId)
            .ToListAsync();

        // ── Step 2: Ensure LeadGridColumn + LeadGridClientSetting ──────────
        bool hasLeadGridColumns = await _context.LeadGridColumns
            .AnyAsync(x => x.ClientId == clientId && x.Active == true);

        if (!hasLeadGridColumns)
        {
            int displayOrder = 1;

            // "All" tab — no status filter
            var allCol = LeadGridColumn.Create("All", displayOrder++, null, clientId, employeeId, isDefaultStatusTab: true);
            _context.LeadGridColumns.Add(allCol);
            await _context.SaveChangesAsync();
            _context.LeadGridClientSettings.Add(
                LeadGridClientSetting.Create(allCol.LeadGridColumnId, null, clientId, employeeId));
            await _context.SaveChangesAsync();

            // One tab per global status
            foreach (var status in globalStatuses)
            {
                var col = LeadGridColumn.Create(status.Description!, displayOrder++, null, clientId, employeeId);
                _context.LeadGridColumns.Add(col);
                await _context.SaveChangesAsync();
                _context.LeadGridClientSettings.Add(
                    LeadGridClientSetting.Create(col.LeadGridColumnId, status.LeadStatusId.ToString(), clientId, employeeId));
                await _context.SaveChangesAsync();
            }
        }

        // ── Step 3: Ensure ClientLeadStatusLookup ──────────────────────────
        bool hasClientStatuses = await _context.ClientLeadStatusLookups
            .AnyAsync(x => x.ClientId == clientId);

        if (!hasClientStatuses)
        {
            int order = 1;
            foreach (var status in globalStatuses)
            {
                var entity = ClientLeadStatusLookup.Create(
                    status.Description!,
                    order++,
                    clientId,
                    employeeId);
                await _context.ClientLeadStatusLookups.AddAsync(entity);
            }
            await _context.SaveChangesAsync();
        }
    }
}
