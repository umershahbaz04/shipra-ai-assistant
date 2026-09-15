using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ClientAggregate;
public class ClientOrderBox
{
  public int ClientOrderBoxId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public string? BoxName { get; private set; }
  public decimal? Length { get; private set; }
  public decimal? Width { get; private set; }
  public decimal? Height { get; private set; }
  public decimal? Volume { get; private set; }
  public bool? IsDefault { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public bool? Active { get; private set; }

  public static ClientOrderBox Create(ClientId? clientId, string? boxName, decimal? length, decimal? width, decimal? height, decimal? volume,bool? isDefault, EmployeeId employeeId)
  {
    return new ClientOrderBox
    {
      ClientId = clientId,
      BoxName = boxName,
      Length = length,
      Width = width,
      Height = height,
      Volume = volume,
      CreatedBy = employeeId,
      IsDefault = isDefault,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public void EnableDisable(bool? isActive, EmployeeId employeeId)
  {
    Active = isActive;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void MarkDefault()
  {
    IsDefault = true;
  }

  public void RemoveDefault()
  {
    IsDefault = false; 
  }

  public void Update(string? boxName, decimal? length, decimal? width, decimal? height, decimal? volume, bool? isDefault, EmployeeId employeeId)
  {
    BoxName = boxName;
    Length = length;
    Width = width; Height = height;
    Volume = volume;
    IsDefault = isDefault;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
}
