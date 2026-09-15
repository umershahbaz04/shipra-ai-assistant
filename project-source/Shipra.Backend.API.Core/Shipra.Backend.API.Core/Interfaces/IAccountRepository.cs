using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.AccountAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IAccountRepository
{
  Task<CarrierPaymentSettlement> CreateCarrierPaymentSettlement(CarrierPaymentSettlement model);
  Task<CarrierPaymentSettlement> UpdateCarrierPaymentSettlement(CarrierPaymentSettlement model);
  Task<CarrierPaymentSettlement?> GetCarrierPaymentSettlementById(CarrierPaymentSettlementId carrierPaymentId); 
  Task<dynamic> GetAllCarrierPaymentSettlements(string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<dynamic> GetShipmentsBySettlementId(string carrierPaymentSettlementId, string clientId);
  #region Pop file 
  Task<CPSettlementPopFile> CreateCpsettlementPopFile(CPSettlementPopFile oCpsettlementPodFile);
  Task<CPSettlementPopFile> GetCpsettlementPopFile(int cpsettlementPopFileId, CarrierPaymentSettlementId carrierPaymentSettlementId);
  Task<bool> DeleteCpsettlementPopFile(CPSettlementPopFile oCpsettlementPodFile);
  Task<List<CPSettlementPopFile>> GetAllCpsettlementPopFiles(CarrierPaymentSettlementId carrierPaymentSettlementId);
  Task<bool> DeleteCarrierPaymentSettlement(CarrierPaymentSettlement oCarrierPaymentSettlement);
  Task<dynamic> GetAllCarrierWithCodPending(DateTime? createdFrom, DateTime? createdTo, string clientId);
  #endregion

}
