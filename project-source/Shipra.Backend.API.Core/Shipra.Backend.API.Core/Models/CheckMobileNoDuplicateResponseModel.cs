namespace Shipra.Backend.API.Core.Models;

public class CheckMobileNoDuplicateResponseModel
{
    public bool IsDuplicate { get; set; }
    public string? OrderNo { get; set; }
    public int DaysAgo { get; set; }
    public string? MobileNo { get; set; }
}
