namespace Shipra.Backend.API.Core.Enum;

public enum InventoryTransactionType
{
    Receipt = 1,
    Allocation = 2,
    Deallocation = 3,
    Pick = 4,
    Pack = 5,
    Dispatch = 6,
    Return = 7,
    Adjustment = 8,
    Damage = 9,
    TransferOut = 10,
    TransferIn = 11,
    CycleCount = 12,
    ExternalSync = 13
}
