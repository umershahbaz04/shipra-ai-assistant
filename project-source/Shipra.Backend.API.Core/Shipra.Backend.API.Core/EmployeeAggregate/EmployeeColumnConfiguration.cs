using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.EmployeeAggregate;
public class EmployeeColumnConfiguration
{
  public int ColumnConfigurationId { get; set; }
  public EmployeeId? EmployeeId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? TableName { get; set; }
  public string? Config { get; set; } // JSON stored as string 
  public DateTime? CreatedOn { get; set; }
  public DateTime? UpdatedOn { get; set; }

  public static EmployeeColumnConfiguration Create(EmployeeId employeeId, ClientId clientId, string? tableName, string? config)
  {
    return new EmployeeColumnConfiguration()
    {
      EmployeeId = employeeId,
      ClientId = clientId,
      TableName = tableName,
      Config = config,
      CreatedOn = DateTime.UtcNow
    };
  }
  public void Update(string? config)
  {
    Config = config;
    UpdatedOn = DateTime.UtcNow;
  }
}
