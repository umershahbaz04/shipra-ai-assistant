using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ShipperInvoiceUserCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetServiceRateGroupById;
public class GetServiceRateGroupByIdQuery : IRequest<ServiceResultDTO>
{
  public int ServiceRateGroupId { get; set; }

  // ================= HANDLER =================
  public class GetServiceRateGroupByIdQueryHandler : RequestHandlerBase<GetServiceRateGroupByIdQuery, ServiceResultDTO>
  {
    private readonly IShipperInvoiceRepository _repo;

    public GetServiceRateGroupByIdQueryHandler(IShipperInvoiceRepository repo, IServiceProvider serviceProvider, ILogger<GetServiceRateGroupByIdQueryHandler> logger) : base(serviceProvider, logger)
    {
      _repo = repo;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetServiceRateGroupByIdQuery request, CancellationToken cancellationToken)
    {
      var result = new ServiceResultDTO();

      try
      {
        var serviceRateGroup = await _repo.GetServiceRateGroupById(request.ServiceRateGroupId,_currentUser.ClientId!.Value);

        if (serviceRateGroup == null)
        {
          result.CreateError("NotFound", new[] { "ServiceRateGroup not found." });
          return result;
        }
        var slabs = await _repo.GetAllServiceRateGroupSlabByServiceRateGroupId(serviceRateGroup.ServiceRateGroupId);

        // Map to Edit DTO (ONLY editable fields)
        var dto = new ServiceRateGroupEditDto
        {
          ServiceRateGroupId = serviceRateGroup.ServiceRateGroupId,
          From = serviceRateGroup.From,
          To = serviceRateGroup.To,
          OriginTypeId = serviceRateGroup.OriginTypeId,
          CalculationMethodId = serviceRateGroup.CalculationMethodId,
          Unit = serviceRateGroup.Unit,
          AdditionalRate = serviceRateGroup.AdditionalRate,
          Code = serviceRateGroup.Code,
          ServiceTypeId = serviceRateGroup.ServiceTypeId,
          EntityFromAddress = serviceRateGroup.EntityFromAddress,
          EntityToAddress = serviceRateGroup.EntityToAddress,

          Slabs = slabs.OrderBy(x => x.WeightFrom).Select(x => new ServiceRateGroupSlabEditDto
                {
                  ServiceRateGroupSlabId = x.ServiceRateGroupSlabId,
                  WeightFrom = x.WeightFrom,
                  WeightTo = x.WeightTo,
                  Rate = x.Rate
                }).ToList()
        };

        result = new ServiceResultDTO(dto);
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
  public class GetServiceRateGroupByIdQueryValidator : AbstractValidator<GetServiceRateGroupByIdQuery>
  {
    public GetServiceRateGroupByIdQueryValidator()
    {
      RuleFor(x => x.ServiceRateGroupId).NotNull().NotEmpty().GreaterThan(0);
    }
  }
}
