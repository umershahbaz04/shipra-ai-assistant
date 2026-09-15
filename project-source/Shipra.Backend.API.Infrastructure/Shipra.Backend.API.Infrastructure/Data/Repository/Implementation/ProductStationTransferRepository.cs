using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ProductStationTransferRepository : IProductStationTransferRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public ProductStationTransferRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  public async Task<dynamic> CreateProductStationTransfer(ProductStationTransfer productStationTransfer, List<TransferProduct> transferProducts)
  {
    _context.ProductStationTransfers.Add(productStationTransfer);
    _context.TransferProducts.AddRange(transferProducts);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<List<ProductStationTransfer>?> GetAllProductStationTransfer(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {
    return await _context.ProductStationTransfers.ToListAsync();
  }
  public async Task<dynamic> GetAllTypeCountProductStationTransfer(string clientId)
  {

    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var AllCount = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM ProductStaionTransfer");
      var PendingCount = await connection.ExecuteScalarAsync($"SELECT COUNT(*) FROM ProductStaionTransfer where TransferStatusId = {(int)EnumProductStaionTransferStatus.Pending}");
      var CompletedCount = await connection.ExecuteScalarAsync($"SELECT COUNT(*) FROM ProductStaionTransfer where TransferStatusId = {(int)EnumProductStaionTransferStatus.Completed}");

      var res =
      new
      {
        AllCount,
        PendingCount,
        CompletedCount
      };
      return res;
    }
  }
  public async Task<ProductStationTransfer?> GetProductStationTransferById(ProductStaionTransferId productStaionTranferId)
  {
    return await _context.ProductStationTransfers.FirstOrDefaultAsync(x => x.ProductStaionTransferId! == productStaionTranferId);
  }

  public async Task<List<TransferProduct>?> GetTransferProductByProductStationTransferId(ProductStaionTransferId productStationTransferId)
  {
    return await _context.TransferProducts.Where(x => x.ProductStaionTransferId == productStationTransferId).ToListAsync();
  }
}
