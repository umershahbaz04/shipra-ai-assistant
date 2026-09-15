using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.WalletAggregate;
public class PayoutFile
{
  public PayoutFileId? PayoutFileId { get; set; }
  public PayoutId? PayoutId { get; set; }
  public string? FilePath { get; set; }
  public DateTime? CreatedOn { get; set; }


  public static PayoutFile Create(PayoutId? payoutId, string? filePath)
  {
    return new PayoutFile
    {
      PayoutFileId = PayoutFileId.New,
      PayoutId = payoutId,
      FilePath = filePath,
      CreatedOn = DateTime.UtcNow,
    };
  }
}
public sealed record PayoutFileId(Guid Value)
{
  public static PayoutFileId New => new(Guid.NewGuid());
}

