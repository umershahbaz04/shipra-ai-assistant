using System;
using System.Collections.Generic;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;

public partial class ServiceRateGroup
{
  public int ServiceRateGroupId { get; private set; }
  public int? From { get; private set; }
  public int? To { get; private set; }
  public int? OriginTypeId { get; private set; }
  public int? CalculationMethodId { get; private set; }
  public decimal? Unit { get; private set; }
  public decimal AdditionalRate { get; private set; }
  public string? Code { get; private set; }
  public int? ServiceTypeId { get; private set; }
  public Guid? ClientId { get; set; }
  public DateTime CreatedOn { get; private set; }
  public Guid? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public Guid? UpdatedBy { get; private set; }
  public string? EntityFromAddress { get; private set; }
  public string? EntityToAddress { get; private set; }

  public static ServiceRateGroup Create(int? from,int? to,int? originTypeId,decimal? unit,decimal additionalRate, string? code,int? serviceTypeId,string? entityFromAddress,string? entityToAddress,Guid? clientId,Guid createdBy)
  {
    return new ServiceRateGroup
    {
      From = from,
      To = to,
      OriginTypeId = originTypeId,
      CalculationMethodId = (int)EnumCalculationMethod.Slab,
      Unit = unit,
      AdditionalRate = additionalRate,
      Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim(),
      ServiceTypeId = serviceTypeId,
      EntityFromAddress = NormalizeNullable(entityFromAddress),
      EntityToAddress = NormalizeNullable(entityToAddress),
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
  }

  public void Update(int? from,int? to,int? originTypeId, int? calculationMethodId,decimal? unit,decimal additionalRate,string? code,int? serviceTypeId, string? entityFromAddress,string? entityToAddress,Guid updatedBy)
  {
     
    From = from;
    To = to;
    OriginTypeId = originTypeId;
    CalculationMethodId = calculationMethodId;
    Unit = unit;
    AdditionalRate = additionalRate;
    Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim();
    ServiceTypeId = serviceTypeId;
    EntityFromAddress = NormalizeNullable(entityFromAddress);
    EntityToAddress = NormalizeNullable(entityToAddress);

    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  private static string? NormalizeNullable(string? value)
      => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

