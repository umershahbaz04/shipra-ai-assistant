namespace Shipra.Backend.API.Core.Enum;
public enum EnumSaleChannelLookup
{ 
  Shopify = 1,
  WooCommerce = 2,
  SalePerson = 100,
  Amazon = 3,
  Noon = 5,
  Wix = 6,
  OpenCart = 7,
  Magento2 = 8,
  BigCommerce = 9,
  PrestaShop = 10,
  eBay = 11,
  Salla = 12,
  Zid = 13,
  YouCan = 14,
  Odoo = 15,
  SAP = 16,
  QuickBook = 17,
  Tally = 18
}
public static class EnumSaleChannelLookupHelper
{
  public static string GetEnumString(EnumSaleChannelLookup value)
  {
    switch (value)
    {
      case EnumSaleChannelLookup.Shopify:
        return "Shopify";
      case EnumSaleChannelLookup.WooCommerce:
        return "WooCommerce";
      case EnumSaleChannelLookup.SalePerson:
        return "SalePerson";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumSaleChannelLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumSaleChannelLookup.Shopify;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
