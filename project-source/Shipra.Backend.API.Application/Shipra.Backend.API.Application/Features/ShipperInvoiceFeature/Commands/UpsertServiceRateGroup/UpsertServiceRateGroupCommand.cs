using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.UpsertServiceRateGroup;
public class UpsertServiceRateGroupCommand : IRequest<ServiceResultDTO>
{
  // Group
  public int? ServiceRateGroupId { get; set; } // 0=create, >0=update 
  public string? From { get; set; }
  public string? To { get; set; }
  public int? OriginTypeId { get; set; }
  public int? CalculationMethodId { get; set; } // for update only (your entity Update requires it)
  public decimal? Unit { get; set; }
  public decimal AdditionalRate { get; set; }
  public string? Code { get; set; }
  public int? ServiceTypeId { get; set; }
  public Dictionary<string, string>? AddressFrom { get; set; }
  public Dictionary<string, string>? AddressTo { get; set; }

  // Slabs
  public List<SlabDto> Slabs { get; set; } = new();

  public class SlabDto
  {
    public int? ServiceRateGroupSlabId { get; set; }
    public decimal? WeightFrom { get; set; }
    public decimal? WeightTo { get; set; }
    public decimal? Rate { get; set; }
  }

  // ================= HANDLER =================
  public class UpsertServiceRateGroupCommandHandler : RequestHandlerBase<UpsertServiceRateGroupCommand, ServiceResultDTO>
  {
    private readonly IShipperInvoiceRepository _repo;

    public UpsertServiceRateGroupCommandHandler(IShipperInvoiceRepository repo, IServiceProvider serviceProvider, ILogger<UpsertServiceRateGroupCommandHandler> logger) : base(serviceProvider, logger)
    {
      _repo = repo;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(UpsertServiceRateGroupCommand request, CancellationToken cancellationToken)
    {
      var result = new ServiceResultDTO();

      try
      {
        // Split comma separated From/To
        var fromValues = (request.From ?? "")
          .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
          .Select(int.Parse)   // convert to int
          .ToList();

        var toValues = (request.To ?? "")
          .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
          .Select(int.Parse)   // convert to int
          .ToList();
        #region MyRegion
        foreach (var from in fromValues)
        {
          var fromaddress = JsonConvert.SerializeObject(
              FilterAddress(request.AddressFrom, from.ToString())
          );

          foreach (var to in toValues)
          {
            var toAddress = JsonConvert.SerializeObject(
                FilterAddress(request.AddressTo, to.ToString())
            );
            List<ServiceRateGroup> allServiceGroupList = await _repo.GetAllServiceRateGroup(_currentUser.ClientId!.Value);

            ServiceRateGroup? carrierRate = allServiceGroupList
                  .FirstOrDefault(x =>
                      x.From == from && x.To == to && x.Code == request.Code );
            if (carrierRate is null || request.ServiceRateGroupId.GetValueOrDefault() > 0)
            {

              // 🔍 Find existing
               carrierRate = allServiceGroupList
                  .FirstOrDefault(x => x.ServiceRateGroupId == request.ServiceRateGroupId  );

              if (carrierRate == null)
              {
                // ✅ CREATE
                carrierRate = ServiceRateGroup.Create(
                   from: from,
                   to: to,
                   originTypeId: request.OriginTypeId,
                   unit: request.Unit,
                   additionalRate: request.AdditionalRate,
                   code: request.Code,
                   serviceTypeId: request.ServiceTypeId,
                   entityFromAddress: fromaddress,
                   entityToAddress: toAddress,
                   clientId: _currentUser.ClientId!.Value,
                   createdBy: _currentUser.EmployeeId!.Value
               );

                await _repo.CreateServiceRateGroup(carrierRate);

                // 🔑 VERY IMPORTANT — keep list in sync
                allServiceGroupList.Add(carrierRate);
              }
              else
              {
                // ✅ UPDATE
                carrierRate.Update(
                    from: from,
                    to: to,
                    originTypeId: request.OriginTypeId,
                    calculationMethodId: request.CalculationMethodId,
                    unit: request.Unit,
                    additionalRate: request.AdditionalRate,
                    code: request.Code,
                    serviceTypeId: request.ServiceTypeId,
                    entityFromAddress: fromaddress,
                    entityToAddress: toAddress,
                    updatedBy: _currentUser.EmployeeId!.Value
                );

                await _repo.UpdateServiceRateGroup(carrierRate);
              }

              // 🔹 SLABS UPSERT
              if (request.CalculationMethodId == 2 && request.Slabs != null)
              {
                foreach (var item in request.Slabs)
                {
                  var existingSlab =
                      await _repo.GetServiceRateGroupSlabByRange(
                          carrierRate.ServiceRateGroupId,
                          item.WeightFrom.GetValueOrDefault(),
                          item.WeightTo.GetValueOrDefault()
                      );

                  if (existingSlab == null)
                  {
                    var slab = ServiceRateGroupSlab.Create(
                        serviceRateGroupId: carrierRate.ServiceRateGroupId,
                        weightFrom: item.WeightFrom.GetValueOrDefault(),
                        weightTo: item.WeightTo.GetValueOrDefault(),
                        rate: item.Rate
                    );

                    await _repo.CreateServiceRateGroupSlab(slab);
                  }
                  else
                  {
                    existingSlab.Update(
                        item.WeightFrom.GetValueOrDefault(),
                        item.WeightTo.GetValueOrDefault(),
                        item.Rate
                    );

                    await _repo.UpdateServiceRateGroupSlab(existingSlab);
                  }
                }
              }
            }
          }
        }

        #endregion

        result = new ServiceResultDTO();

        return result;
      }
      catch (Exception ex)
      {
        result.CreateErrorResponse(ex);
        return result;
      }
    }

  }

  // ================= VALIDATOR =================
  public class UpsertServiceRateGroupCommandValidator : AbstractValidator<UpsertServiceRateGroupCommand>
  {
    public UpsertServiceRateGroupCommandValidator()
    {
      RuleFor(x => x.OriginTypeId).GreaterThan(0);
      RuleFor(x => x.ServiceTypeId).GreaterThan(0);

      RuleFor(x => x.AdditionalRate).GreaterThanOrEqualTo(0);

      RuleFor(x => x.Code).MaximumLength(50);

      RuleFor(x => x.Slabs)
          .NotNull()
          .Must(x => x.Count > 0)
          .WithMessage("At least one slab is required.");

      RuleForEach(x => x.Slabs).ChildRules(s =>
      {
        s.RuleFor(x => x.WeightFrom).GreaterThanOrEqualTo(0);
        s.RuleFor(x => x.WeightTo).GreaterThan(0);
        s.RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
      });
    }
  }
  public static Dictionary<string, string> FilterAddress(Dictionary<string, string>? addressDict, string filterValue)
  {
    if (addressDict == null || addressDict.Count == 0)
      return new Dictionary<string, string>();
    var lastKey = addressDict.Keys.Last();
    var lastValue = addressDict[lastKey];

    var values = lastValue.Split(',', StringSplitOptions.RemoveEmptyEntries);

    if (values.Contains(filterValue))
    {
      var result = new Dictionary<string, string>(addressDict);
      result[lastKey] = filterValue;
      return result;
    }

    return new Dictionary<string, string>(); // no match
  }
}
