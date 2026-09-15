using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.SMSProcessAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ISMSProcessRepository
{
  Task<dynamic> GetAllSMSActivate(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId);
  Task<SMSActivate> CreateSMSActivate(SMSActivate ppactivate);
  Task<SMSActivate?> GetSMSActivateById(int ppactivateId, ClientId clientId);
  Task<dynamic?> GetSMSActivateForSelection(string clientId);
  Task<dynamic> DeleteSMSActivate(SMSActivate ppactivate);
  Task<dynamic> UpdateSMSActivate(SMSActivate ppactivate);
  Task<SMSLookup?> GetSMSLookupById(int pPLookupId);
  Task<List<SMSLookup>?> GetAllSMSLookupForSelection();
  Task<SMSActivate?> GetSMSActivateBySMSLookupId(int pplookupId, ClientId clientId);
  Task<SMSActivate?> GetClientDefaultSMSActivateById(ClientId clientId);
  Task<List<SMSActivate>?> GetAllDefaultSmsActivated(ClientId clientId);
  Task<List<SMSActivate>?> GetAllSmsActivated(ClientId clientId);
  #region whatsapp
  Task<WhatsappLookup?> GetWhatsappLookupById(int pPLookupId);
  Task<WhatsappActivate?> GetWhatsappActivateById(int ppactivateId, ClientId clientId);
  Task<WhatsappActivate> CreateWhatsappActivate(WhatsappActivate ppactivate);
  Task<dynamic> UpdateWhatsappActivate(WhatsappActivate whatsappActivate);
  Task<List<WhatsappActivate>?> GetAllDefaultWhatsappActivated(ClientId clientId);
  Task<List<WhatsappActivate>?> GetAllWhatsappActivated(ClientId clientId);
  Task<dynamic> GetAllWhatsappActivate(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId);
  Task<List<WhatsappLookup>?> GetAllWhatsappLookupForSelection();
  #endregion

  #region WhatsApp Button Handler
  Task<WhatsappActivate?> GetWhatsappActivationById(int sMSActivateId, ClientId clientId);
  #endregion
}
