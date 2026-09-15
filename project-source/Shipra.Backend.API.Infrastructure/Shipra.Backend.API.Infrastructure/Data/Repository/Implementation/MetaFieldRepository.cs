using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.ExpenseAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.MetaFieldAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class MetaFieldRepository : IMetaFieldRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;
  public MetaFieldRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<List<EntityMetaFieldLookup>> GetMetaFields()
  {
    var data = await _context.EntityMetaFieldLookups.ToListAsync();
    return data;
  }

  public async Task<MetaField?> GetMetaFieldsByOrderIdAsync(string orderId)
  {
    return await _context.MetaFields
        .SingleOrDefaultAsync(x => x.EntityId == orderId);
  }

  public async Task<List<MetaField>> GetMetaFieldsByOrderIdsAsync(List<string> orderIds)
  {
    return await _context.MetaFields
        .Where(x => orderIds.Contains(x.EntityId!))
        .ToListAsync();
  }

  public async Task<ClientMetaField> CreateClientMetaField(ClientMetaField clientmetadata)
  {
    await _context.ClientMetaFields.AddAsync(clientmetadata);
    await _context.SaveChangesAsync();
    return clientmetadata;
  }
  public async Task<ClientMetaField?> GetClientMetaDataById(int? metafieldId, ClientId clientId)
  {
    return await _context.ClientMetaFields.Where(x => x.ClientMetaFieldId == metafieldId && x.ClientId == clientId).FirstOrDefaultAsync();
  }
  public async Task<dynamic> UpdateClientMetaData(ClientMetaField ClMetadata)
  {
    _context.ClientMetaFields.Update(ClMetadata);
    await _context.SaveChangesAsync();
    return ClMetadata;
  }
  public async Task<List<ClientMetaField>> GetClientMetaDataByClientId(ClientId clientId)
  {
    return await _context.ClientMetaFields.Where(x =>x.ClientId == clientId).ToListAsync();
  }
  public async Task<MetaField> CreateMetaFields(MetaField metadata)
  {
    try
    {
      await _context.MetaFields.AddAsync(metadata);
      await _context.SaveChangesAsync();
      return metadata;
    }
    catch (Exception)
    {

      throw;
    }
     
  }
  public async Task<MetaField?> GetMetaFieldDataByOrderId(string Orderid)
  {
    return await _context.MetaFields.Where(x => x.EntityId == Orderid).FirstOrDefaultAsync();
  }
  public async Task<dynamic> UpdateMetaFieldData(MetaField Metadata)
  {
    _context.MetaFields.Update(Metadata);
    await _context.SaveChangesAsync();
    return Metadata;
  }
}
