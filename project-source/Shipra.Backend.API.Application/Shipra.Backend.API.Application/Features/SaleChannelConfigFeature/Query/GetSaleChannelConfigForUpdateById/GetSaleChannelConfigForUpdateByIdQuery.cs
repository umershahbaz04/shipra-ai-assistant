using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigForUpdateById;
public class GetSaleChannelConfigForUpdateByIdQuery :IRequest<ServiceResultDTO>
{
  public int SaleChannelConfigId { get; set; }
}
public class GetSaleChannelConfigForUpdateByIdQueryHandler : RequestHandlerBase<GetSaleChannelConfigForUpdateByIdQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;

  public GetSaleChannelConfigForUpdateByIdQueryHandler(ISaleChannelConfigRepository SaleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<GetSaleChannelConfigForUpdateByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _saleChannelConfigRepository = SaleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSaleChannelConfigForUpdateByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSCConfig = await _saleChannelConfigRepository.GetSaleChannelConfigForUpdateById(request.SaleChannelConfigId, _currentUser.ClientId!);
      if (oSCConfig is null)
      {
        throw new EntityNotFoundException("SaleChannelConfig ", request.SaleChannelConfigId);
      }
      if (string.IsNullOrEmpty(oSCConfig!.Config!))
      {
        throw new EntityNotFoundException("SaleChannelConfig ", oSCConfig!.SaleChannelConfigId!);
      }

      serviceResult = new ServiceResultDTO(oSCConfig!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetSaleChannelConfigForUpdateByIdQueryValidator : AbstractValidator<GetSaleChannelConfigForUpdateByIdQuery>
{
  public GetSaleChannelConfigForUpdateByIdQueryValidator()
  {
    RuleFor(x => x.SaleChannelConfigId).NotEmpty().GreaterThan(0);
  }
}
