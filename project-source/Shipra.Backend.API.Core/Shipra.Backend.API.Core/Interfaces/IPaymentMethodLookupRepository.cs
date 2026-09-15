using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IPaymentMethodLookupRepository
{
  Task<List<PaymentMethodLookup>?> GetAllPaymentMethodLookup();

}
