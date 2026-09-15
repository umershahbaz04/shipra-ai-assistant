using System.Dynamic;
using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class EmployeeRepository : IEmployeeRepository
{
  private readonly ICatalogueRepository _catalogueRepository;
  private readonly AppDbContext _context;
  private readonly DapperAppDbContext _dapperAppDbContext;

  public EmployeeRepository(ICatalogueRepository catalogueRepository, AppDbContext context, DapperAppDbContext dapperAppDbContext)
  {
    _catalogueRepository = catalogueRepository;
    _context = context;
    _dapperAppDbContext = dapperAppDbContext;
  }

  public async Task<Driver?> CheckEmployeeExists(EmployeeId? employeeId)
  {
    return await _context.Drivers.FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Active == true);
  }

  public async Task<Employee> CreateEmployee(Employee employee)
  {
    await _context.Employees.AddAsync(employee);
    await _context.SaveChangesAsync();
    return employee;
  }
  public async Task<int> GetEmployeeCountByClient(ClientId clientId)
  {
    return await _context.Employees.CountAsync(x => x.ClientId == clientId);
  }

  public async Task<dynamic> DeleteEmployee(Employee employee)
  {
    _context.Employees.Update(employee);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> GetAllEmployees(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, int? userRoleId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY e.CreatedOn) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               e.EmployeeId,
                               e.EmployeeName,
                               gl.GenderName,
                               e.DateOfBirth,
                               e.EmployeeImage,
                               e.EmployeeCode,
                               c.Username,
                               e.MobileNo,
                               e.PhoneNo,
                               ISNULL(cur.RoleName, '') AS RoleName,
                               e.WorkEmail,
                               e.Active,
                               e.IsClient,
							                 ISNULL(et.EmployeeTypeName, 'Employee') As EmployeeTypeName
                        FROM dbo.Employee AS e
                            LEFT JOIN dbo.GenderLookup AS gl
                                ON e.GenderId = gl.GenderId
                            INNER JOIN dbo.Client AS c
                                ON c.ClientId = e.ClientId
                            LEFT JOIN dbo.ClientUserRole AS cur
                                ON cur.ClientUserRoleId = e.ClientUserRoleId 
							          LEFT JOIN dbo.EmployeeType AS et
                                ON et.EmployeeTypeId = e.EmployeeTypeId ";

      string whereStart = "WHERE ( 1= 1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( e.EmployeeCode in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (c.ClientId = @ClientId) ";
      }
      if (userRoleId != null && userRoleId > 0)
      {
        dynamicParams.Add("@userRoleId", userRoleId);
        whereStart += "And (e.ClientUserRoleId = @userRoleId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("e.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("e.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }


      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "e.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }

  }

  public async Task<Employee> GetEmployeeById(EmployeeId employeeId, ClientId clientId)
  {
    var target = await _context.Employees.FirstOrDefaultAsync(x => x.Active == true && x.ClientId! == clientId && x.EmployeeId == employeeId);
    return target!;
  }
  public async Task<Employee> GetEmployeeByIdForEdit(EmployeeId employeeId, ClientId clientId)
  {
    var target = await _context.Employees.FirstOrDefaultAsync(x => x.ClientId! == clientId && x.EmployeeId == employeeId);
    return target!;
  }

  public async Task<List<EmployeeType>> GetAllEmployeeTypesAsync()
  {
    return await _context.EmployeeTypes.ToListAsync();
  }
  public async Task<List<Employee>> GetAllSalesPersonForSelection(ClientId? clientId)
  {
    var target = await _context.Employees.Where(x => x.Active == true && x.ClientId == clientId && x.SaleChannelConfigId != null && x.SaleChannelConfigId != 0).ToListAsync();
    return target!;
  }
  public async Task<Employee> GetEmployeeBySaleChannelConfigId(int saleChannelConfigId, ClientId? clientId)
  {
    var target = await _context.Employees.FirstOrDefaultAsync(x => x.ClientId! == clientId && x.SaleChannelConfigId == saleChannelConfigId && x.Active == true);
    return target!;
  }
  public async Task<List<Employee>?> GetAllEmployeesForSelection(ClientId? clientId)
  {
    var target = await _context.Employees.Where(x => x.Active == true && x.ClientId == clientId).ToListAsync();
    return target;
  }

  public async Task<dynamic> GetEmployeesForShipperContract(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"
            SELECT 
                CAST(e.EmployeeId AS NVARCHAR(40)) AS EmployeeId,
                e.SaleChannelConfigId,
                e.EmployeeCode,
                s.StoreName,
                e.EmployeeName,
                CONCAT(
                    e.EmployeeCode, ' | ',
                    s.StoreName, ' | ',
                    e.EmployeeName
                ) AS EmployeeDisplay
            FROM dbo.Employee AS e
            INNER JOIN dbo.SaleChannelConfig AS scc
                ON scc.SaleChannelConfigId = e.SaleChannelConfigId
            INNER JOIN dbo.Stores AS s
                ON s.StoreId = scc.StoreId
        ";

      string whereStart = $"WHERE ( e.Active = 1 AND e.EmployeeTypeId = {(int)EnumEmployeeType.Shipper} ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " AND scc.ClientId = @ClientId ";
      }

      string where = whereStart + whereEnd;
      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data!;
    }
  }


  public async Task<dynamic> GetEmployeeProfileById(string employeeId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT e.EmployeeId,
                       e.EmployeeName,
                       gl.GenderName,
                       e.DateOfBirth,
                       e.EmployeeImage,
                       e.EmployeeCode,
                       e.Latitude,
                       e.Longitude,
                       e.MobileNo,
                       e.PhoneNo,
                       e.RegionId,
                       e.Zip,
                       e.WorkEmail,
                       e.CityId,
                       e.AddressLine1,
                       e.AddressLine2,
                       e.Active,
                       e.CountryId,
                       e.Zip,
                       e.Latitude,
                       e.Longitude,
                       cn.Name AS CountryName,
                       r.Name AS RegionName,
                       ct.Name AS CityName
                FROM dbo.Employee AS e
					          LEFT JOIN dbo.GenderLookup AS gl 
					              ON e.GenderId=gl.GenderId
                    LEFT JOIN dbo.Country AS cn
                        ON e.CountryId = cn.CountryId
                    LEFT JOIN dbo.Region AS r
                        ON e.RegionId = r.RegionId
                    LEFT JOIN dbo.City AS ct
                        ON e.CityId = ct.CityId
		                INNER JOIN dbo.Client AS c ON c.ClientId = e.ClientId  ";
      string whereStart = "WHERE ( e.Active=1 ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " AND  c.ClientId = @ClientId ";
      }
      if (!string.IsNullOrEmpty(employeeId))
      {
        dynamicParams.Add("@employeeId", employeeId);
        whereStart += " AND e.EmployeeId = @employeeId ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data!;
    }

  }

  public async Task<dynamic> UpdateEmployee(Employee employee)
  {
    _context.Employees.Update(employee);
    return await _context.SaveChangesAsync() > 0;
  }
  #region employee code

  #region new algo 
  public async Task<string> GetEmployeeNextCodeByUserName(string? employeeName, ClientId clientId, EmployeeId employeeId)
  {
    if (clientId is null || employeeId is null)
      throw new ArgumentNullException("ClientId or EmployeeId cannot be null.");

    int? clientIdentifier = 0;
    var client = await _context.Clients
        .Where(x => x.ClientId == clientId && x.Active == true)
        .FirstOrDefaultAsync();

    if (client is not null)
      clientIdentifier = client.ClientIdentifier;

    string employeeCode = await GenerateEmployeeCodeWithCatLogAsync(employeeName, clientId, clientIdentifier);

    return employeeCode;
  }
  private async Task<string> GenerateEmployeeCodeWithCatLogAsync(string? employeeName, ClientId clientId, int? clientIdentifier)
  {
    var oCatalogue = await _catalogueRepository.GetCatalogByClientId(clientId.Value.ToString());
    if (oCatalogue is null)
      throw new InvalidOperationException("Catalogue not found.");

    int empCount = await GetEmployeeCountByClient(clientId);
    empCount = empCount + 1;
    string prefix = UtilityHelper.GenerateEmployeeCode(employeeName!, true) ?? string.Empty;

    // Using StringBuilder for efficient string concatenation
    var sb = new StringBuilder();
    sb.Append(prefix);
    sb.Append(oCatalogue.DatabaseId);
    sb.Append(clientIdentifier ?? 0);
    sb.Append(RandomNumGenerator.GetRandom1NumberFrom1To10());
    sb.Append(empCount);


    string employeeCode = sb.ToString();

    var employee = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeCode == employeeCode);

    if (employee != null)
    {
      // Employee code already exists, generate a new one recursively
      return await GenerateEmployeeCodeWithCatLogAsync(employeeName, clientId, clientIdentifier);
    }

    return employeeCode;
  }
  #endregion
  public async Task<string> GetEmployeeNextCode(ClientId clientId)
  {
    int? clientIdentifier = 0;
    var client = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync();
    if (client is not null)
    {
      clientIdentifier = client.ClientIdentifier;
    }
    string employeeCode = await GenerateEmployeeCodeAsync(clientIdentifier); //EM1001

    return employeeCode;
  }
  private async Task<string> GenerateEmployeeCodeAsync(int? clientIdentifier)
  {
    string employeeCode = $"{clientIdentifier}{GenerateNextEmployeeUserName()}";

    var emp = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeCode == employeeCode);

    if (emp != null)
    {
      // Employee code already exists, generate a new one recursively
      return await GenerateEmployeeCodeAsync(clientIdentifier);
    }

    return employeeCode;
  }
  #region em no
  private string? GenerateNextEmployeeUserName()
  {
    Guid myGuid = Guid.NewGuid();
    uint hashCode = (uint)myGuid.GetHashCode();
    string formattedHex = string.Format("{0:x}", hashCode).Substring(0, 6); ;

    return formattedHex;

  }

  #endregion
  #endregion

  public async Task<List<GenderLookup>> GetAllGenderForSelection()
  {
    return await _context.GenderLookups!.ToListAsync();
  }
  public async Task<List<Employee>> GetAllSaleConfigEmployeesByClient(ClientId clientId)
  {
    return await _context.Employees!.Where(x => x.SaleChannelConfigId.GetValueOrDefault() > 0 && x.ClientId == clientId).ToListAsync();
  }  
  public async Task<List<SaleChannelConfig>> GetAllSaleChannelConfigByEmployess(ClientId clientId)
  {
    var emmployees = await GetAllSaleConfigEmployeesByClient(clientId);
    var configIds = emmployees.Select(x => x.SaleChannelConfigId).ToList();
    return await _context.SaleChannelConfigs!.Where(x => configIds.Contains(x.SaleChannelConfigId)).ToListAsync();
  }



  public async Task<EmployeeAddress> GetEmployeeAddressById(EmployeeId? employeeId)
  {
    var target = await _context.EmployeeAddresses.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
    return target!;
  }

  public async Task<bool> CreateEmployeeAddress(EmployeeAddress oEmployeeAddress)
  {
    await _context.EmployeeAddresses.AddAsync(oEmployeeAddress);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> UpdateEmployeeAddress(EmployeeAddress oEmployeeAddress)
  {
    _context.EmployeeAddresses.Update(oEmployeeAddress);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<string?> GetEmployeeNameById(EmployeeId? employeeId)
  {
    string name = string.Empty;
    var oEmployee = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
    if (oEmployee is not null)
    {
      name = $"{oEmployee.EmployeeName}";
    }
    return name;
  }

  public async Task<bool> CreateEmployeeColumnConfiguration(EmployeeColumnConfiguration oEmployeeColumnConfiguration)
  {
    await _context.EmployeeColumnConfigurations.AddAsync(oEmployeeColumnConfiguration);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> UpdateEmployeeColumnConfiguration(EmployeeColumnConfiguration oEmployeeColumnConfiguration)
  {
    _context.EmployeeColumnConfigurations.Update(oEmployeeColumnConfiguration);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<EmployeeColumnConfiguration>> GetAllEmployeeColumnConfiguration(ClientId? clientId)
  {
    var target = await _context.EmployeeColumnConfigurations.Where(x => x.ClientId == clientId).ToListAsync();
    return target!;
  }
}
