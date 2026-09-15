using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shipra.Backend.API.Application.Services.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data;
public class DapperAppDbContext
{
  private readonly IConfiguration _configuration;
  private readonly ShipraMasterDbContext _shipraMasterDbContext;
  private readonly IKeyGeneratorService _keyGeneratorService;
  private readonly string? _connectionString;
  private readonly string? _shipperInvoiceConnection;

  public DapperAppDbContext(IConfiguration configuration, ShipraMasterDbContext shipraMasterDbContext, IKeyGeneratorService keyGeneratorService)
  {
    _configuration = configuration;
    _shipraMasterDbContext = shipraMasterDbContext;
    _keyGeneratorService = keyGeneratorService;
    _connectionString = _configuration.GetConnectionString("ShipraMasterConnection");
    _shipperInvoiceConnection = _configuration.GetConnectionString("ShipperInvoiceConnection");
  }
  public IDbConnection CreateConnection()
  => new SqlConnection(_connectionString);  
  public IDbConnection CreateShipperInvoiceConnection()
  => new SqlConnection(_shipperInvoiceConnection);

  public IDbConnection CreateConnectionByEncryptedConString(string encryptedConnectionString)
  {
    var connectionString = _keyGeneratorService.DecryptString(encryptedConnectionString) ?? string.Empty;
    return new SqlConnection(connectionString);
  }
  public IDbConnection CreateConnectionByClient(string clientId)
  {
    return new SqlConnection(GetConnectionString(clientId));
  }
  public IDbConnection CreateConnectionByClientOrMasterDb(bool isFromMaster = false, string? clientId = null)
  {
    if (isFromMaster)
    {
      return new SqlConnection(_connectionString);
    }
    else
    {
      return new SqlConnection(GetConnectionString(clientId!));
    }
  }
  public string GetConnectionString(string clientId)
  {
    string connectionString = string.Empty;
    if (!string.IsNullOrEmpty(clientId))
    {
      clientId = clientId.Trim();
      var oCatalogues = _shipraMasterDbContext.Catalogues.FirstOrDefault(x => x.ClientId == clientId);
      if (oCatalogues is not null)
      {
        var oCatalogueDatabases = _shipraMasterDbContext.CatalogueDatabases.FirstOrDefault(x => x.DatabaseId == oCatalogues!.DatabaseId && x.Active == true);
        connectionString = "Server=13.205.115.20,1245; initial catalog=App_Shipra; MultipleActiveResultSets= True; user id=sa;password=devshipra@123;multipleactiveresultsets=True;TrustServerCertificate=True";//_keyGeneratorService.DecryptString(oCatalogueDatabases?.ConnectionString!) ?? string.Empty;
      }
    }
    return connectionString;
  }
}
