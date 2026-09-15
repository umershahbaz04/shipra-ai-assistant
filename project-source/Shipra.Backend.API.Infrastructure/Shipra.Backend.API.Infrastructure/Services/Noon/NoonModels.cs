using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Shipra.Backend.API.Infrastructure.Services.Noon;

public class NoonAuthResponse
{
    [JsonProperty("token")]
    public string? Token { get; set; }
}

public class NoonStockRequest
{
    [JsonProperty("items")]
    public List<NoonStockItem>? Items { get; set; }
}

public class NoonStockItem
{
    [JsonProperty("warehouse_code")]
    public string? WarehouseCode { get; set; }

    [JsonProperty("partner_sku")]
    public string? PartnerSku { get; set; }
}

public class NoonStockResponse
{
    [JsonProperty("items")]
    public List<NoonStockResponseItem>? Items { get; set; }
}

public class NoonStockResponseItem
{
    [JsonProperty("partner_sku")]
    public string? PartnerSku { get; set; }

    [JsonProperty("qty")]
    public int Quantity { get; set; }
    
    [JsonProperty("warehouse_code")]
    public string? WarehouseCode { get; set; }
}

public class NoonOrderListRequest
{
    [JsonProperty("warehouse_code")]
    public string? WarehouseCode { get; set; }

    [JsonProperty("created_after")]
    public string? CreatedAfter { get; set; }

    [JsonProperty("created_before")]
    public string? CreatedBefore { get; set; }
}

public class NoonOrderListResponse
{
    [JsonProperty("items")]
    public List<NoonOrderListItem>? Items { get; set; }
}

public class NoonOrderListItem
{
    [JsonProperty("fbpi_order_nr")]
    public string? FbpiOrderNr { get; set; }
    
    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }
}

public class NoonFbpiOrderResponse
{
    [JsonProperty("fbpi_order_nr")]
    public string? FbpiOrderNr { get; set; }

    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonProperty("warehouse_code")]
    public string? WarehouseCode { get; set; }
    
    [JsonProperty("country_code")]
    public string? CountryCode { get; set; }

    [JsonProperty("items")]
    public List<NoonFbpiOrderItem>? Items { get; set; }
}

public class NoonFbpiOrderItem
{
    [JsonProperty("partner_sku")]
    public string? PartnerSku { get; set; }
    
    [JsonProperty("mp_item_nr")]
    public string? MpItemNr { get; set; }

    [JsonProperty("price")]
    public decimal Price { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }
}

public class NoonCustomerDetailsResponse
{
    [JsonProperty("customer_name")]
    public string? CustomerName { get; set; }

    [JsonProperty("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonProperty("address")]
    public NoonAddress? Address { get; set; }
}

public class NoonAddress
{
    [JsonProperty("address_line1")]
    public string? AddressLine1 { get; set; }

    [JsonProperty("address_line2")]
    public string? AddressLine2 { get; set; }

    [JsonProperty("city")]
    public string? City { get; set; }

    [JsonProperty("state")]
    public string? State { get; set; }

    [JsonProperty("country_code")]
    public string? CountryCode { get; set; }
}
