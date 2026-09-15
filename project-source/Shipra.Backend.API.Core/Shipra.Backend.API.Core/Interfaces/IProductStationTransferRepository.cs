using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IProductStationTransferRepository
{
  Task<dynamic> CreateProductStationTransfer(ProductStationTransfer productStationTransfer, List<TransferProduct> transferProduct);
  Task<List<ProductStationTransfer>?> GetAllProductStationTransfer(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<dynamic> GetAllTypeCountProductStationTransfer(string clientId);
  Task<ProductStationTransfer?> GetProductStationTransferById(ProductStaionTransferId productStationTranferId);
  Task<List<TransferProduct>?> GetTransferProductByProductStationTransferId(ProductStaionTransferId productStationTransferId);
}
