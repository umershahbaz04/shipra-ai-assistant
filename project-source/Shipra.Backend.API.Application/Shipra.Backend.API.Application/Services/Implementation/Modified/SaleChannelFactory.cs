using Microsoft.Extensions.DependencyInjection;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Services.Implementation.Modified;

public class SaleChannelFactory : ISaleChannelFactory
{
  private readonly IServiceProvider _serviceProvider;

  public SaleChannelFactory(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public ISaleChannelService GetSaleChannelService(int saleChannelLookupId)
  {
    return saleChannelLookupId switch
    {
      (int)EnumSaleChannelLookup.Shopify => _serviceProvider.GetRequiredService<ShopifyService>(),
      //(int)EnumSaleChannelLookup.WooCommerce => _serviceProvider.GetRequiredService<WooCommerceService>(),
      _ => throw new NotImplementedException($"Sale channel {saleChannelLookupId} is not supported")
    };
  }
}

