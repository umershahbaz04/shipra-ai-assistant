using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Ocsp;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.OrderBoxAggregate;
public partial class OrderBox
{
  public OrderBoxId? OrderBoxId { get; private set; }
  public int? ClientOrderBoxId { get; private set; }
  public OrderId? OrderId { get; private set; }
  public decimal? Length { get; private set; }
  public decimal? Width { get; private set; }
  public decimal? Height { get; private set; }
  public decimal? Volume { get; private set; }
  public static OrderBox Create(int? clientOrderBoxId, OrderId orderId)
  {
    return new OrderBox
    {
      OrderBoxId = OrderBoxId.New,
      ClientOrderBoxId = clientOrderBoxId,
      OrderId = orderId
    };
  }
  public static OrderBox Create(decimal? length, decimal? width, decimal? height, OrderId orderId)
  { 
    return new OrderBox
    {
      OrderBoxId = OrderBoxId.New,
      Length = length,
      Width = width,
      Height = height,
      Volume = CalculateVolume(length, width, height),
      OrderId = orderId
    };
  }
  public void Update(int? clientOrderBoxId)
  {
    ClientOrderBoxId = clientOrderBoxId;
  } 
  public void Update(decimal? length, decimal? width, decimal? height)
  {
    Length = length;
    Width = width;
    Height = height;
    Volume = CalculateVolume(length, width, height);
  }

  private static decimal? CalculateVolume(decimal? length, decimal? width, decimal? height)
  {
    if (!length.HasValue || !width.HasValue || !height.HasValue)
      return null;

    if (length <= 0 || width <= 0 || height <= 0)
      return null;

    return (length.Value * width.Value * height.Value)
           / UtilityHelper.WeightDimensionalFactor;
  }

}
public sealed record OrderBoxId(Guid Value)
{
  public static OrderBoxId New => new(Guid.NewGuid());
}
