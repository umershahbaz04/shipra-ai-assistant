using System.Dynamic;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Stripe;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class StripeRepository : IStripeRepository
{
  public async Task<dynamic> CreateInvoice(Order order, List<OrderItem> orderItems, string customerId, string stripeSecretKey)
  {
    try
    {
      // Replace with your Stripe API secret key with customer setting stripe key
      StripeConfiguration.ApiKey = stripeSecretKey;
      var hostedUrl = string.Empty;
      var hostedPDFUrl = string.Empty;
      // Create an invoice item 
      var options = new InvoiceCreateOptions
      {
        Customer = customerId,
        AutoAdvance = false,
        Currency = EnumCurrencyHelper.GetEnumString(EnumCurrency.USD),
        CollectionMethod = "send_invoice",
        DueDate = DateTime.Now.AddDays(30),
        CustomFields = new List<InvoiceCustomFieldOptions>{
          new InvoiceCustomFieldOptions{ Name = "Shipra Order No.", Value = order.OrderNo }
        }
      };
      var invoiceService = new InvoiceService();
      var invoiceCreateOptions = await invoiceService.CreateAsync(options);
      if (!string.IsNullOrEmpty(invoiceCreateOptions.Id))
      {
        var invoiceItemService = new InvoiceItemService();
        foreach (var item in orderItems)
        {
          // Create a line item
          var invoiceItemOptions = new InvoiceItemCreateOptions
          {
            Customer = customerId,
            UnitAmountDecimal = item.Price * 100,
            Currency = EnumCurrencyHelper.GetEnumString(EnumCurrency.USD),
            Description = item.Description,
            Invoice = invoiceCreateOptions.Id
          };
          await invoiceItemService.CreateAsync(invoiceItemOptions);
        }
        var sendInvoice = await invoiceService.SendInvoiceAsync(invoiceCreateOptions.Id);
        dynamic result = new ExpandoObject();
        result.HostedInvoiceUrl = sendInvoice.HostedInvoiceUrl;
        result.InvoicePdf = sendInvoice.InvoicePdf;
        result.Id = sendInvoice.Id;
        return result;
      }
      else
      {
        throw new Exception("Request Failed while processing Stripe");
      }
    }
    catch (Exception ex)
    {
      throw new Exception("Request Failed while processing Stripe", ex.InnerException);
    }
  }

  public async Task<string?> CreateCustomerAsync(string name, string phone, string? email = null, string? stripeSecretKey = null)
  {
    try
    {
      // Replace with your Stripe API secret key with customer setting stripe key
      StripeConfiguration.ApiKey = stripeSecretKey;
      // Create the customer options
      var options = new CustomerCreateOptions
      {
        Name = name,
        Email = email,
        Phone = phone
      };

      // Create the customer
      var service = new CustomerService();
      var customer = await service.CreateAsync(options);

      // Handle the customer response
      return customer.Id;
    }
    catch (Exception ex)
    {
      throw new Exception("Request Failed while processing Stripe", ex.InnerException);
    }
  }

  public async Task<string?> ValidateStripeAccount(string stripeSecretKey)
  {
    try
    {
      // Replace with your Stripe API secret key with account setting stripe key
      StripeConfiguration.ApiKey = stripeSecretKey;
      var accountService = new AccountService();
      var account = await accountService.GetSelfAsync();

      // Handle the customer response
      return account.Id;
    }
    catch (Exception ex)
    {
      throw new Exception($"Request Failed while processing Stripe: Error {ex.InnerException}");
    }
  }

  public async Task<dynamic> CreateStripeWebhookAsync(string baseURL, List<string> events, string stripeSecretKey)
  {
    try
    {
      // Replace with your Stripe API secret key with webhook setting stripe key
      StripeConfiguration.ApiKey = stripeSecretKey;
      var webhookService = new WebhookEndpointService();
      // Create a webhook endpoint in Stripe
      var options = new WebhookEndpointCreateOptions
      {
        Url = baseURL + "/api/webhook",
        EnabledEvents = events
      };
      var webhook = await webhookService.CreateAsync(options);

      // Handle the webhook response
      return webhook;
    }
    catch (Exception ex)
    {
      throw new Exception($"Request Failed while processing Stripe: Error {ex.InnerException}");
    }
  }

  public async Task<dynamic> GetStripeWebhookAsync(string webhookId, string stripeSecretKey)
  {
    try
    {
      // Replace with your Stripe API secret key with webhook setting stripe key
      StripeConfiguration.ApiKey = stripeSecretKey;
      var webhookService = new WebhookEndpointService();
      // Create a webhook endpoint in Stripe
      var webhookList = await webhookService.ListAsync();
      var webhookEndPoint = webhookList.Data.FirstOrDefault(x => x.Id == webhookId);
      if (webhookEndPoint != null)
      {
        // Handle the webhook response
        return webhookEndPoint;
      }
      else
      {
        return new WebhookEndpoint();
      }
    }
    catch (Exception ex)
    {
      throw new Exception($"Request Failed while processing Stripe: Error {ex.InnerException}");
    }
  }
}
