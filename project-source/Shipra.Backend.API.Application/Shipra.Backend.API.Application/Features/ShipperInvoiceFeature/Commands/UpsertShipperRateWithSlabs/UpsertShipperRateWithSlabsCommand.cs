using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.UpsertShipperRateWithSlabs;
public class UpsertShipperRatesWithSlabsCommand : IRequest<ServiceResultDTO>
{
  public List<ShipperRateItem> Items { get; set; } = new();

  public class ShipperRateItem
  {
    public long? ShipperRateId { get; set; } = 0; // 0=create, >0=update
    public int? SaleChannelConfigId { get; set; }
    public string? EmployeeName { get; set; }
    public int? ServiceRateGroupId { get; set; }
    public decimal? AdditionalRate { get; set; }
    public long? ContractShipperRatesId { get; set; }
    public int? From { get; set; }
    public int? To { get; set; }

    public List<ShipperRateSlabDto> Slabs { get; set; } = new();
  }

  public class ShipperRateSlabDto
  {
    public int? ShipperRateSlabId { get; private set; }
    public int? ServiceRateGroupSlabId { get; set; }
    public decimal? Rate { get; set; }
  }

  // ================= HANDLER =================
  public class UpsertShipperRatesWithSlabsCommandHandler : RequestHandlerBase<UpsertShipperRatesWithSlabsCommand, ServiceResultDTO>
  {
    private readonly IShipperInvoiceRepository _repo;

    public UpsertShipperRatesWithSlabsCommandHandler(IShipperInvoiceRepository repo, IServiceProvider serviceProvider, ILogger<UpsertShipperRatesWithSlabsCommandHandler> logger) : base(serviceProvider, logger)
    {
      _repo = repo;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(UpsertShipperRatesWithSlabsCommand request, CancellationToken cancellationToken)
    {
      var result = new ServiceResultDTO();

      try
      {
        if (request.Items == null || request.Items.Count == 0)
        {
          result.CreateError("Validation", new[] { "Items cannot be null or empty." });
          return result;
        }
        bool IsSameRange(ShipperRateItem x) =>
      x.From.HasValue &&
      x.To.HasValue &&
      x.From.Value == x.To.Value;

        // Get all items where From == To
        var sameRangeItems = request.Items
            .Where(IsSameRange)
            .ToList();

        List<ShipperRateItem> invalidItems = new();
        List<ShipperRateItem> validItems = request.Items.ToList();

        // If more than one record has From == To → mark them invalid
        if (sameRangeItems.Count > 1)
        {
          invalidItems = sameRangeItems;
          request.Items = request.Items
              .Where(x => !IsSameRange(x))
              .ToList();
        }


        var clientId = _currentUser.ClientId!.Value;
        var employeeId = _currentUser.EmployeeId!.Value;

        var createdIds = new List<long>();
        var updatedIds = new List<long>();
        var slabsProcessed = 0;
        var saleChannelConfigId = request.Items.FirstOrDefault();

        if (saleChannelConfigId is null)
          throw new Exception("SaleChannelConfigId is required.");

        await _repo.DeleteShipperRatesAsync(
            saleChannelConfigId!.SaleChannelConfigId!.GetValueOrDefault(),
            _currentUser.ClientId.Value
        );


        foreach (var item in request.Items)
        {
          // ================= CREATE =================
          //if (item.ShipperRateId.GetValueOrDefault() == 0)
          //{
          ShipperRate? shipperRate = ShipperRate.Create(
                clientId: clientId,
                saleChannelConfigId: item.SaleChannelConfigId,
                serviceRateGroupId: item.ServiceRateGroupId,
                additionalRate: item.AdditionalRate,
                contractShipperRatesId: item.ContractShipperRatesId,
                employeeName: item.EmployeeName,
                createdBy: employeeId
            );

          var saved = await _repo.CreateShipperRate(shipperRate);
          if (saved is not null)
          {
            shipperRate = saved;
            createdIds.Add(shipperRate.ShipperRateId);
          }
          //}
          //// ================= UPDATE =================
          //else
          //{
          //  shipperRate = await _repo.GetShipperRateById(item.ShipperRateId.GetValueOrDefault(), clientId, item.SaleChannelConfigId.GetValueOrDefault());

          //  if (shipperRate is not null)
          //  {
          //    shipperRate.Update(additionalRate: item.AdditionalRate,contractShipperRatesId: item.ContractShipperRatesId,active: shipperRate.Active, updatedBy: employeeId);

          //    var ok = await _repo.UpdateShipperRate(shipperRate);
          //    if (ok)
          //    {
          //      updatedIds.Add(shipperRate.ShipperRateId);
          //    }
          //  }
          //}

          // Safety
          if (shipperRate is not null)
          {

            // ================= SLABS UPSERT =================
            foreach (var slabDto in item.Slabs)
            {
              var existingSlab = await _repo.GetShipperRateSlab(shipperRate.ShipperRateId, slabDto.ShipperRateSlabId.GetValueOrDefault());

              if (existingSlab != null)
              {
                existingSlab.Update(slabDto.Rate);

                var ok = await _repo.UpdateShipperRateSlab(existingSlab);
              }
              else
              {
                var slab = ShipperRateSlab.Create(shipperRateId: shipperRate.ShipperRateId, serviceRateGroupSlabId: slabDto.ServiceRateGroupSlabId, rate: slabDto.Rate);

                var created = await _repo.CreateShipperRateSlab(slab);
              }

              slabsProcessed++;
            }
          }
        }
        result = new ServiceResultDTO(new
        {
          Created = createdIds,
          Updated = updatedIds,
          SlabsProcessed = slabsProcessed
        });

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
  public class Validator : AbstractValidator<UpsertShipperRatesWithSlabsCommand>
  {
    public Validator()
    {
      RuleFor(x => x.Items)
          .NotNull()
          .Must(x => x.Count > 0)
          .WithMessage("Items cannot be null or empty.");

      RuleForEach(x => x.Items).ChildRules(i =>
      {
        i.RuleFor(x => x.SaleChannelConfigId).GreaterThan(0);
        i.RuleFor(x => x.ServiceRateGroupId).GreaterThan(0);

        i.RuleForEach(x => x.Slabs).ChildRules(s =>
        {
          s.RuleFor(x => x.ServiceRateGroupSlabId).GreaterThan(0);
          s.RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
        });

        // prevent duplicate slab ids inside a single item request
        //i.RuleFor(x => x.Slabs)
        //    .Must(slabs => slabs.Select(s => s.ServiceRateGroupSlabId).Distinct().Count() == slabs.Count)
        //    .WithMessage("Duplicate ServiceRateGroupSlabId found in slabs list.");
      });
    }
  }
}
