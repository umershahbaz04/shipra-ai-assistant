using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.MetaFieldAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IMetaFieldRepository
{
  Task<List<EntityMetaFieldLookup>> GetMetaFields();
  Task<ClientMetaField> CreateClientMetaField(ClientMetaField clientmetadata);
  Task<ClientMetaField?> GetClientMetaDataById(int? metafieldId, ClientId clientId);
  Task<dynamic> UpdateClientMetaData(ClientMetaField ClMetadata);
  Task<MetaField?> GetMetaFieldsByOrderIdAsync(string OrderId);
  Task<List<MetaField>> GetMetaFieldsByOrderIdsAsync(List<string> orderIds);
  Task<List<ClientMetaField>> GetClientMetaDataByClientId(ClientId clientId);
  Task<MetaField> CreateMetaFields(MetaField metadata);
  Task<MetaField?> GetMetaFieldDataByOrderId(string Orderid);
  Task<dynamic> UpdateMetaFieldData(MetaField Metadata);
}
