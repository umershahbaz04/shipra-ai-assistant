using Ardalis.Specification.EntityFrameworkCore;
using Dapper;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.ExpenseAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class CommonLookupRepository : ICommonLookupRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public CommonLookupRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context, IShipmentStatusCommonRepository shipmentStatusCommonRepository)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  public async Task<List<AddressTypeLookup>?> GetAllAddressTypeLookup()
  {
    return await _context.AddressTypeLookups.ToListAsync();
  }

  public async Task<List<ExpenseCategory>?> GetAllExpenseCategories(ClientId clientId)
  {
    return await _context.ExpenseCategories.Where(x => x.ClientId == clientId).ToListAsync();
  }

  public async Task<List<FullFillmentStatusLookup>?> GetAllFullFillmentStatusLookup()
  {
    return await _context.FullFillmentStatusLookups.ToListAsync();
  }

  public async Task<List<LookupAdjustReason>?> GetAllLookupAdjustReason()
  {
    return await _context.LookupAdjustReasons
        .FromSqlRaw("SELECT InventoryTransactionTypeId AS LookupAdjustReasonId, Name AS Reason FROM dbo.InventoryTransactionTypeLookup WHERE Active = 1")
        .ToListAsync();
  }

  public async Task<dynamic> GetAllONGFTypeLookp()
  {
    return await _context.ONGFTypeLookups.ToListAsync();
  }
  public async Task<List<PaymentStatusLookup>?> GetAllPaymentStatusLookup()
  {
    return await _context.PaymentStatusLookups.ToListAsync();
  }
  public async Task<List<ProductOptionLookup>?> GetAllProductOptionLookup()
  {
    return await _context.ProductOptionLookups.ToListAsync();
  }
  public async Task<List<ScfolderLookup>?> GetAllSCFolderLookup()
  {
    return await _context.ScfolderLookups.ToListAsync();
  }
  public async Task<List<WhatsAppCategoryType>?> GetAllWhatsAppCategoryLookup()
  {
    return await _context.WhatsAppCategoryTypeLookups.ToListAsync();
  }
  public async Task<List<CarrierTrackingStatusLookup>?> GetAllCarrierTrackingStatusLookup()
  {
    return await _context.CarrierTrackingStatusLookups.ToListAsync();
  }
  public async Task<List<ClientCarrierTrackingStatus>?> GetAllClientCarrierTrackingStatus(ClientId clientId)
  {
    return await _context.ClientCarrierTrackingStatuses.Where(x => x.ClientId == clientId).ToListAsync();
  }
  public async Task<dynamic?> GetCompletedShipmentGridSetting(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string queryData = $@"SELECT sgcs.DashboardStatusValue
                                       FROM dbo.ShipmentGridClientSetting AS sgcs
                                       WHERE sgcs.ShipmentGridColumnId =
                                       (
                                           SELECT sgc.ShipmentGridColumnId
                                           FROM dbo.ShipmentGridColumn AS sgc
                                           WHERE sgc.ColumnName = 'COMPLETED'
                                                 AND sgc.ClientId = '{clientId}'
                                       )  ";
         
      var data = await connection.QueryAsync(queryData, dynamicParams);
      var dashboardStatusValue = data.ToList();
      string statusValue = "";
      if (dashboardStatusValue.Count > 0)
      {
        var obj = dashboardStatusValue.FirstOrDefault();  
        if (obj != null)
        {
          statusValue = obj.DashboardStatusValue;
        }
      }
      return statusValue;
    }
  }
}
