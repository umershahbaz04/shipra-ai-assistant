using System.ComponentModel;

namespace Shipra.Backend.API.Core.Enum;

public enum EnumWipeOutSection
{
    [Description("All Client Data")]
    All = 1,

    [Description("Products and Inventory")]
    Products = 2,

    [Description("Orders and Shipments")]
    Orders = 3,

    [Description("Returns and Exchanges")]
    Returns = 4,

    [Description("Delivery Notes and Tasks")]
    Delivery = 5
}
