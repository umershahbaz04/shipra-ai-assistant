using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ClientLoginRepository : IClientLoginRepository
{ 
  public async Task<ClientResponseModel?> GetClientByClientId(string clientId, string connectionString)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT c.ClientCode,
       c.ClientName,
       c.ClientImage,
       c.ClientCompanyName,
       ca.CountryId,
       co.Name AS CountryName,
       ca.StreetAddress,
       ca.Zip,
       c.Mobile,
       c.Phone,
       c.Email,
       c.LicenseNo,
       c.ClientIdentifier,
       c.DefaultCarrierId,
       c.RegionTimeZoneId,
       c.Username,
       ISNULL(c.AllowWithoutBalance, 0) AS AllowWithoutBalance,
       ISNULL(c.AllowPersonalClientCarrierContract, 0) AS AllowPersonalClientCarrierContract,
	   ISNULL(ccs.AllowShipperInvocie,0) AS AllowShipperInvocie,
       c.PublicKey,
       c.SecretKey,
       c.EncryptedKey,
       CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.ClientMetaField WHERE ClientId = c.ClientId) THEN 1 ELSE 0 END AS BIT) AS IsShowMetafield
FROM dbo.Client AS c
    LEFT JOIN dbo.ClientConfigSetting AS ccs
        ON ccs.ClientId = c.ClientId
    INNER JOIN dbo.ClientAddress AS ca
        ON ca.ClientId = c.ClientId
    INNER JOIN dbo.Country AS co
        ON co.CountryId = ca.CountryId ";
      string whereStart = $"WHERE ( c.Active=1 AND ( c.ClientId = '{clientId}' ) ";
      string whereEnd = ")";

      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync<ClientResponseModel>(queryData, dynamicParams);
      return data.FirstOrDefault();
      }

    }

  public async Task<EmployeeResponseModel?> GetEmployeebyId(string clientId, string employeeId, string connectionString)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT e.EmployeeName,
                       e.MobileNo as Mobile,
                       ISNULL(e.PhoneNo,'') AS Phone,
                       e.WorkEmail as Email
                FROM dbo.Employee AS e ";
      string whereStart = $"WHERE ( e.Active=1 AND ( e.ClientId = '{clientId}' )  AND ( e.EmployeeId = '{employeeId}' )  ";
      string whereEnd = ")";

      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync<EmployeeResponseModel>(queryData, dynamicParams);
      return data.FirstOrDefault();
    }

  }
  public async Task<ClientConfigSettingDto?> GetClientConfigSetting(string clientId,string connectionString)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT * FROM dbo.ClientConfigSetting AS ccs ";
      string whereStart = $"WHERE (  ccs.ClientId = '{clientId}'  ";
      string whereEnd = ")";
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync<ClientConfigSettingDto>(queryData, dynamicParams);
      return data.FirstOrDefault();
    }
  }
}
