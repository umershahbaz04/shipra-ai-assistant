using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.PaymentProcessAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IPaymentProcessRepository
{
  Task<dynamic> GetAllPPActivate(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId);
  Task<Ppactivate> CreatePPActivate(Ppactivate ppactivate);
  Task<Ppactivate?> GetPPActivateByPPActivateId(int ppactivateId, ClientId clientId);
  Task<dynamic> DeletePPActivate(Ppactivate ppactivate);
  Task<dynamic> UpdatePPActivate(Ppactivate ppactivate);
  Task<Pplookup?> GetPPLookupById(int pPLookupId);
  Task<List<Pplookup>?> GetAllPPLookupForSelection();
  Task<List<Ppactivate>?> GetAllDefaultPPActivated(ClientId clientId);
  Task<Ppactivate?> GetPPActivateByPPLookupId(int pplookupId, ClientId clientId);
  Task<Ppactivate?> GetClientDefaultPaymentProcessById(ClientId clientId);
}
