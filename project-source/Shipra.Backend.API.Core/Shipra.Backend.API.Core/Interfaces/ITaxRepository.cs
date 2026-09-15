using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.TaxAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ITaxRepository
{
  Task<List<TaxTypeLookup>> GetAllTaxTypeLookup();
  Task<TaxTypeLookup?> GetTaxTypeLookupById(int TaxId);
  #region client tax 
  Task<ClientTax?> CreateClientTax(ClientTax model);
  Task<bool> UpdateClientTax(ClientTax model);
  Task<ClientTax?> GetClientTaxById(int clientTaxId,ClientId clientId); 
  Task<ClientTax?> GetAddedTaxById(int taxId,ClientId clientId); 
  Task<dynamic> GetAllClientTaxes(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string? clientId);
  #endregion

}
