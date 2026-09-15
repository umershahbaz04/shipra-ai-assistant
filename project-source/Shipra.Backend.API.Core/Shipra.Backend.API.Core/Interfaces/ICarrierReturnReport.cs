using Shipra.Backend.API.Core.CarrierReturnReportAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ICarrierReturnReport
{
  Task<CarrierReturnReport> CreateCarrierReturnReport(CarrierReturnReport report);
  Task<CarrierReturnReport> GetCarrierReturnRerport(CarrierRRId carrierRrid);
  Task<bool> DeleteCarrierReturnRerport(CarrierReturnReport report);
  Task<dynamic> GetReturnRerports(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId);
  Task<dynamic> GetShipmentsByReturnReportId(string carrierRrid, string clientId);
  Task<dynamic> GetAllMyCarrierReturnReport(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, int? carrierId,string clientId);
  Task<CarrierReturnReport?> GetCarrierReturnReportById(CarrierRRId carrierRRId);
}
