using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductStation
{
  public int ProductStationId { get; private set; }
  public string? StationCode { get;private  set; }
  public string? Name { get; private set; }
  public ClientId? ClientId { get; private set; }
  public int? ProductStationTypeId { get; private set; }
  public int? SaleChannelConfigId { get; private set; }
  public string? ExternalStationId { get; private set; }
  public bool? IsShipraManaged { get; private set; }
  public bool? Active { get; private set; }
  public bool? IsDefault { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }


  public static ProductStation AddDefault()
  {
    return new ProductStation()
    {
      ProductStationId = 0,
      Name = ShipraConstants.DropDownPlaceHolderName
    };
  }

  public static ProductStation CreateProductStation(string? stationCode, string? name, ClientId clientId, EmployeeId createdBy, bool? isDefault = false, int? productStationTypeId = 1, int? saleChannelConfigId = null, string? externalStationId = null, bool? isShipraManaged = true)
  {
    var station = new ProductStation()
    {
      StationCode = stationCode,
      Name = name,
      ClientId = clientId,
      ProductStationTypeId = productStationTypeId,
      SaleChannelConfigId = saleChannelConfigId,
      ExternalStationId = externalStationId,
      IsShipraManaged = isShipraManaged,
      Active = true,
      IsDefault = isDefault,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy
    };
    return station;
  }

  public void MarkAsActive(EmployeeId? userId)
  {
    Active = true;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void MarkAsInActive(EmployeeId? userId)
  {
    Active = false;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateProductStation(int productStationId, string? name, EmployeeId updateBy,bool? isDefault=false)
  {
    ProductStationId = productStationId;
    IsDefault = isDefault;
    Name = name;
    UpdatedBy = updateBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void RemoveDefaultProductStation(EmployeeId? userId)
  {
    IsDefault = false;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void MarkAsDefaultProductStation(EmployeeId? userId)
  {
    IsDefault = true;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }
}
