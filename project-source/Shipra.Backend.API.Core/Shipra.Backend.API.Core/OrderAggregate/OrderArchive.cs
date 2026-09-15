using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderArchive
{
  public long OrderArchiveId { get; set; }
  public OrderId? OrderId { get; set; }
  public string? OrderJson { get; set; } = null!;
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public ClientId? ClientId { get; set; }
  public string? ArchiveNo { get; set; }



  public static OrderArchive Create(OrderId? orderId, string? orderJson, EmployeeId? createdBy, ClientId? clientId,string? archiveNo)
  {
    return new OrderArchive
    {
      OrderId = orderId,
      OrderJson = orderJson,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      ClientId = clientId,
      ArchiveNo = archiveNo
    };
  }
  public static string GenerateRandomArchiveNo(int length = 8)
  {
    Random _random = UtilityHelper.GetRandomNumber();
    const string digits = "0123456789";
    return new string(Enumerable.Repeat(digits, length)
      .Select(s => s[_random.Next(s.Length)]).ToArray());
  }

}
