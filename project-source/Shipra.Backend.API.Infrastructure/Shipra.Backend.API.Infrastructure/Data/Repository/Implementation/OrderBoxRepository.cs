using System.Xml.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class OrderBoxRepository : IOrderBoxRepository
{
  private readonly IDbContextService _dbContextService;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;
  public OrderBoxRepository(DapperAppDbContext dapperAppDbContext, IDbContextService dbContextService, AppDbContext context)
  {
    _dbContextService = dbContextService;
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  #region client order box

  public async Task<ClientOrderBox?> CreateClientOrderBox(ClientOrderBox model)
  {
    await _context.ClientOrderBoxes.AddAsync(model);
    await _context.SaveChangesAsync();
    return model;
  }



  public async Task<List<ClientOrderBox>> GetAllClientClientOrderBox(ClientId? clientId)
  {
    return await _context.ClientOrderBoxes.Where(x => x.ClientId == clientId).ToListAsync();
  }
  public async Task<ClientOrderBox?> GetClientOrderBoxById(int clientOrderBoxId, ClientId clientId)
  {
    return await _context.ClientOrderBoxes.FirstOrDefaultAsync(x => x.ClientOrderBoxId == clientOrderBoxId && x.ClientId == clientId);
  }
  public async Task<ClientOrderBox?> GetDefaultClientOrderBoxById(ClientId clientId)
  {
    return await _context.ClientOrderBoxes.FirstOrDefaultAsync(x => x.IsDefault == true && x.ClientId == clientId);
  }

  public async Task<bool> UpdateClientOrderBox(ClientOrderBox model)
  {
    _context.ClientOrderBoxes.Update(model);
    return await _context.SaveChangesAsync() > 0;
  }
  #endregion
  #region MyRegion

  public async Task<List<OrderBox>> GetOrderBoxsByOrderId(OrderId? OrderId)
  {
    return await _context.OrderBoxes.Where(x => x.OrderId == OrderId).ToListAsync();
  }
  public async Task<bool> CreateOrderBox(OrderBox orderBox)
  {
    await _context.OrderBoxes.AddAsync(orderBox);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> UpdateOrderBox(OrderBox orderBox)
  {
    _context.OrderBoxes.Update(orderBox);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> DeleteOrderBox(OrderBox orderBox)
  {
    _context.OrderBoxes.Remove(orderBox);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> IsOrderBoxExist(string boxName, ClientId clientId)
  {
    return await _context.ClientOrderBoxes.AnyAsync(x => x.BoxName!.Trim().ToLower() == boxName.Trim().ToLower() && x.ClientId == clientId);
  }

  public async Task<List<BoxTypeLookup>> GetAllBoxTypeLookup()
  {
    return await _context.BoxTypeLookups.ToListAsync();

  }

  #endregion
}
