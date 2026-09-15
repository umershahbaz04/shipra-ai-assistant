using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IStripeRepository
{
  Task<dynamic> CreateInvoice(Order order, List<OrderItem> orderItems, string customerId, string stripeSecretKey);
  Task<string?> CreateCustomerAsync(string name, string phone, string? email = null, string? stripeSecretKey = null);
  Task<string?> ValidateStripeAccount(string stripeSecretKey);
  Task<dynamic> CreateStripeWebhookAsync(string baseURL, List<string> events, string stripeSecretKey);
  Task<dynamic> GetStripeWebhookAsync(string webhookId, string stripeSecretKey);
}
