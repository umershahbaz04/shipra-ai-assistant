using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class DriverRepository : IDriverRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;
  public DriverRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<Driver?> CreateDriver(Driver driver)
  {
    await _context.Drivers.AddAsync(driver);
    await _context.SaveChangesAsync();
    return driver;
  }

  public async Task<bool> DeleteDriver(Driver driver)
  {
    _context.Drivers.Update(driver);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> GetAllDrivers(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               d.DriverId,
                               d.DriverCode,
	                           e.EmployeeId,
                               e.EmployeeName AS DriverName,
                               e.EmployeeImage,
                               ISNULL(gl.GenderName, '') AS Gender,
                               ISNULL(e.DateOfBirth, '') AS DOB,
                               e.PhoneNo AS Phone,
                               e.MobileNo,
                               e.WorkEmail AS Email
                        FROM dbo.Driver AS d
                            INNER JOIN dbo.Employee AS e
                                ON e.EmployeeId = d.EmployeeId
                            INNER JOIN dbo.GenderLookup AS gl
                                ON gl.GenderId = e.GenderId ";
      string whereStart = "WHERE ( d.active=1 ";
      string whereEnd = ") ";

      if (clientId is not null)
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (d.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@CreatedOn", createdFrom);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("d.CreatedOn", regionMinuts)} AS DATE) = CAST(@CreatedOn AS DATE) )";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@CreatedTo", createdTo);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("d.CreatedOn", regionMinuts)} AS DATE) <= CAST(@CreatedTo AS DATE))";
      }
      string where = whereStart + whereEnd;
      string queryData = query + where;
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

  public async Task<dynamic?> GetAllDriversForSelection(ClientId clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT 
                      d.DriverId,
                      e.EmployeeName as DriverName
                      FROM dbo.Driver AS d 
                      INNER JOIN dbo.Employee  AS e ON e.EmployeeId = d.EmployeeId ";
      string whereStart = "WHERE ( d.active=1 ";
      string whereEnd = ") ";

      if (clientId is not null)
      {
        dynamicParams.Add("@ClientId", clientId.Value.ToString());
        whereStart += "And (d.ClientId = @ClientId) ";
      }

      string where = whereStart + whereEnd;
      string queryData = query + where;
      dynamic dataList = await connection.QueryAsync(queryData, dynamicParams);
      return dataList;
    }
  }

  public async Task<Driver?> GetDriverById(DriverId driverId)
  {
    return await _context.Drivers.FirstOrDefaultAsync(x => x.DriverId == driverId && x.Active == true);
  }
  public async Task<string> GetDriverNextCode(ClientId clientId)
  {
    int driverCount = 0;
    int? clientIdentifier = 0;
    var client = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync();
    if (client is not null)
    {
      clientIdentifier = client.ClientIdentifier;
      driverCount = await _context.Drivers.CountAsync();
      driverCount++;

    }
    string driverCode = $"DR{clientIdentifier}{driverCount}"; //DR1001
    return driverCode;
  }
  public async Task<Driver?> UpdateDriver(Driver driver)
  {
    _context.Drivers.Update(driver);
    await _context.SaveChangesAsync();
    return driver;
  }

  public async Task<Driver?> GetDriverByEmployeeId(EmployeeId employeeId)
  {
    return await _context.Drivers.FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Active == true);
  }

  public async Task<dynamic> GetDriverProfile(string driverId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT d.DriverId,
                             d.DriverCode,
                             d.AppUsername AS DriverName,
                             e.EmployeeImage AS DriverImage,
                             gl.GenderName AS Gender,
                             e.EmployeeId,
                             e.MobileNo,
                             e.PhoneNo,
                             e.WorkEmail,
                             e.DateOfBirth,
                             c.Name AS CountryName,
                             ea.Latitude,
                             ea.Longitude,
                             ea.StreetAddress,
                             ea.StreetAddress2,
                             ea.FullAddress
                      FROM dbo.Employee AS e
                          INNER JOIN dbo.Driver AS d
                              ON d.EmployeeId = e.EmployeeId
                          INNER JOIN dbo.EmployeeAddress AS ea
                              ON ea.EmployeeId = d.EmployeeId
                          INNER JOIN dbo.Country AS c
                              ON c.CountryId = ea.CountryId
                          INNER JOIN dbo.GenderLookup AS gl
                              ON gl.GenderId = e.GenderId ";
      string whereStart = "WHERE ( d.active=1 ";
      string whereEnd = ") ";
      if (!string.IsNullOrEmpty(driverId))
      {
        dynamicParams.Add("@driverId", driverId);
        whereStart += " AND ( d.DriverId = @driverId ) ";
      }
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.FirstOrDefault()!;
    }
  }
  #region carrier status
  public async Task<bool> DeleteDriverCTSSetting(DriverCTSSettingId? driverCtsid)
  {
    var target = await _context.DriverCTSSettings.FirstOrDefaultAsync(x => x.DriverCTSSettingId == driverCtsid);
    _context.DriverCTSSettings.Remove(target!);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> DeleteDriverCTSSetting(ClientId clientId)
  {
    var target = await _context.DriverCTSSettings.Where(x => x.ClientId == clientId).ToListAsync();
    if (target is not null)
    {
      _context.DriverCTSSettings.RemoveRange(target!);
      return await _context.SaveChangesAsync() > 0;
    }
    return false;
  }
  public async Task<dynamic> GetAllDriverCTSSetting(string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT  COUNT(dcs.DriverCTSId) OVER () AS TotalCount,
                       ctsl.CarrierTrackingStatusId,
                       dcs.DriverCTSId,
                       ctsl.TrackingStatus,
                       ISNULL(ctsl.TrackingStatusAr, '-') AS TrackingStatusAr
                FROM dbo.DriverCTSSetting AS dcs
                    INNER JOIN dbo.ClientCarrierTrackingStatus AS ctsl
                        ON ctsl.CarrierTrackingStatusId = dcs.CarrierTrackingStatusId ";
      string whereStart = "WHERE ( dcs.active=1 ";
      string whereEnd = ") ";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@clientId", clientId);
        whereStart += " AND ( dcs.ClientId = @clientId ) ";
      }

      string where = whereStart + whereEnd;
      string queryData = query + where;
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

  public async Task<DriverCTSSetting> CreateDriverCTSSetting(DriverCTSSetting driverCtssetting)
  {
    await _context.DriverCTSSettings.AddAsync(driverCtssetting);
    await _context.SaveChangesAsync();
    return driverCtssetting;
  }

  public async Task<int> CreateDriverDefaultCTSSetting(ClientId? clientId, EmployeeId employeeId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      //Status Values
      int[] statusValues = new int[] { (int)EnumCarrierTrackingStatus.Delivered, (int)EnumCarrierTrackingStatus.Cancelled, (int)EnumCarrierTrackingStatus.LocationChanged, (int)EnumCarrierTrackingStatus.MobileNotAnswered, (int)EnumCarrierTrackingStatus.MobileSwitchedOff, (int)EnumCarrierTrackingStatus.Refunded,(int)EnumCarrierTrackingStatus.Exchanged};
      int isSaved = 0;
      foreach (var statusId in statusValues)
      {
        string query = $@"INSERT INTO dbo.DriverCTSSetting(DriverCTSId, ClientId, CarrierTrackingStatusId, CreatedBy, CreatedOn, Active) SELECT '{Guid.NewGuid()}', '{clientId?.Value}', cts.CarrierTrackingStatusId ,'{employeeId?.Value}','{DateTime.UtcNow}', 1 FROM dbo.ClientCarrierTrackingStatus AS cts WHERE cts.CarrierTrackingStatusId = {statusId} AND cts.ClientId = '{clientId?.Value}' ;";

        isSaved = await connection.ExecuteAsync(query);
      }
      return isSaved;
    }
  }

  #endregion
}
