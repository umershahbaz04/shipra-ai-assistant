using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.ExpenseAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ICommonLookupRepository
{
  Task<List<AddressTypeLookup>?> GetAllAddressTypeLookup();
  Task<List<CarrierTrackingStatusLookup>?> GetAllCarrierTrackingStatusLookup();
  Task<List<ClientCarrierTrackingStatus>?> GetAllClientCarrierTrackingStatus(ClientId clientId);
  Task<List<ExpenseCategory>?> GetAllExpenseCategories(ClientId clientId);
  Task<List<FullFillmentStatusLookup>?> GetAllFullFillmentStatusLookup();
  Task<List<LookupAdjustReason>?> GetAllLookupAdjustReason();
  Task<dynamic> GetAllONGFTypeLookp();
  Task<List<PaymentStatusLookup>?> GetAllPaymentStatusLookup();
  Task<List<ProductOptionLookup>?> GetAllProductOptionLookup();
  Task<List<ScfolderLookup>?> GetAllSCFolderLookup();
  Task<List<WhatsAppCategoryType>?> GetAllWhatsAppCategoryLookup();
  Task<dynamic?> GetCompletedShipmentGridSetting(string clientId);
}
