using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IShipperInvoiceRepository
{
  Task<int?> GetShipperInvoiceCount(int saleChannelConfigId, Guid clientId);
  Task<int> DeleteShipperRatesAsync(int saleChannelConfigId, Guid clientId);
  // ---------------- ShipperRate ----------------
  Task<ShipperRate?> CreateShipperRate(ShipperRate entity);
  Task<bool> UpdateShipperRate(ShipperRate entity); 
  // Scoped fetch to prevent cross-client update:
  Task<ShipperRate?> GetShipperRateById(long shipperRateId, Guid clientId, int saleChannelConfigId);

  Task<ShipperRateSlab?> GetShipperRateSlab(long shipperRateId, int shipperRateSlabId);
  Task<ShipperRateSlab?> CreateShipperRateSlab(ShipperRateSlab entity);
  Task<bool> UpdateShipperRateSlab(ShipperRateSlab entity);
  Task<List<ShipperRateSlab>> GetAllShipperRateSlabById(long? shipperRateId);
  // ------------ ContractShipperRate ------------
  Task<ContractShipperRate?> CreateContractShipperRate(ContractShipperRate entity);
  Task<bool> UpdateContractShipperRate(ContractShipperRate entity);
  Task<ContractShipperRate?> GetContractShipperRateById(long contractShipperRatesId);
  Task<dynamic> GetAllClientRateAsync(int? start, int length, string? search, string? clientId);

  // ---------- ServiceRateGroupSlab --------------
  Task<ServiceRateGroupSlab?> CreateServiceRateGroupSlab(ServiceRateGroupSlab entity);
  Task<bool> UpdateServiceRateGroupSlab(ServiceRateGroupSlab entity);
  Task<ServiceRateGroupSlab?> GetServiceRateGroupSlabById(int serviceRateGroupSlabId);
  Task<List<ServiceRateGroupSlab>> GetAllServiceRateGroupSlabByServiceRateGroupId(int serviceRateGroupId);
  // ------------ ServiceRateGroup ----------------
  Task<ServiceRateGroup?> CreateServiceRateGroup(ServiceRateGroup entity);
  Task<bool> UpdateServiceRateGroup(ServiceRateGroup entity);
  Task<ServiceRateGroup?> GetServiceRateGroupById(int serviceRateGroupId, Guid? clientId);
  Task<ServiceRateGroupSlab?> GetServiceRateGroupSlabByRange(int? serviceRateGroupId, decimal? weightFrom, decimal? weightTo);

  // ----------- ShipperInvoiceDetail -------------
  Task<ShipperInvoiceDetail?> CreateShipperInvoiceDetail(ShipperInvoiceDetail entity);
  Task<bool> UpdateShipperInvoiceDetail(ShipperInvoiceDetail entity);
  Task<ShipperInvoiceDetail?> GetShipperInvoiceDetailById(long shipperInvoiceDetailId, Guid tenantId);

  // ---------------- ShipperInvoice --------------
  Task<ShipperInvoice?> CreateShipperInvoice(ShipperInvoice entity);
  Task<bool> UpdateShipperInvoice(ShipperInvoice entity);
  Task<ShipperInvoice?> GetShipperInvoiceById(int shipperInvoiceId, Guid tenantId);
  Task<List<dynamic>> GetAllShipperInvoiceDetail(string? clientId, int? shipperInvoiceId);
  Task<ShipperInvoiceAdjustment?> CreateShipperInvoiceAdjustment(ShipperInvoiceAdjustment entity);
  Task<bool> UpdateShipperInvoiceAdjustment(ShipperInvoiceAdjustment entity);

  Task<ShipperInvoiceAdjustment?> GetShipperInvoiceAdjustmentById(int shipperInvoiceAdjustmentId, Guid clientId);
  Task<List<ShipperInvoiceAdjustment>?> GetShipperInvoiceAdjustmentBySCId(int saleChannelConfigId, Guid clientId);
  Task<List<ShipperRateResponseModel>> GetShipperRateBySaleChannelConfig(int? SaleChannelConfigId, string? clientId,int? start,int? length,DateTime? createFrom,DateTime? createTo);

  Task<bool> DeleteShipperInvoiceAdjustment(ShipperInvoiceAdjustment entity);
  Task<dynamic> GetAllServiceRateGroups(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int? sortCol, string? sortDir, string? clientId, Dictionary<string, string>? addressFrom, Dictionary<string, string>? addressTo);
  Task<dynamic> GetAllCarrierRateWithContractDataAsync(int? start, int length, string? search, int? deliveryServiceId, string? clientId, int? saleChannelConfigId);

  Task<bool?> CreateShipperOrder(ShipperOrder entity);
  Task<ShipperOrder?> GetShipperOrderByOrderNo(string? orderNo, Guid clientId);
  Task<bool?> UpdateShipperOrder(ShipperOrder entity);
  Task<List<OrderResponseModel>> GetShipperOrderForInvoices(int? start, int? length, string? search, string? shipperInvoiceId, int? saleChannelConfigId, string? clientId, DateTime? createdFrom = null, DateTime? createdTo = null);
  Task<dynamic> GetAllOrders(int start, int length, string? search, string? orderNos, string? statusIds, string? clientId, DateTime? createdFrom, DateTime? createdTo);
  Task<List<ShipperInvoiceAdjustmentModel>> GetAllShipperInvoiceAdjustment(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? clientId, int TransactionTypeId = 0, int? InvoiceCreateId = 0);
  Task<List<ShipperInvoiceAdjustment>> GetAllDeliveryAdjustmentByIds(List<int>? selectedadjustmentIds, Guid? clientId);
  Task<dynamic> GetAllShipperForGenerateInvocice(string? clientId);
  Task<List<ServiceRateGroup>> GetAllServiceRateGroup(Guid? clientId);
  Task<List<InvoiceStatus>> GetAllInvoiceStatus();
  Task<List<TransactionType>> GetAllTransactionType();
  Task<List<ServiceRateGroup>> GetAllServiceRateGroupForSelection(Guid? clientId);
  Task<ShipperOrder?> GetShiperOrderByOrderNo(string orderNo, Guid? clientId);
  Task<List<ShipperInvoiceListRow>> GetAllShipperInvoice(DateTime? createdFrom, DateTime? createdTo, int? start, int? length, string? search, int? sortCol, string? sortDir, string? clientId, int? transactionTypeId = 0, int? invoiceStatusId = 0, int? saleChannelConfigId = 0);
  Task<List<ShipperInvoiceAdjustment>?> GetShipperInvoiceAdjustmentByShipperInvoiceId(int? shipperInvoiceId, Guid? clientIdStr);
  Task<bool> DeleteShiperOrder(ShipperOrder oShiperOrder);
}

