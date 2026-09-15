using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllOrdersByPayoutId;
public class GetAllOrdersByPayoutIdQuery : CommonOrderFilters, IRequest<ServiceResultDTO>
{
  public string? PayoutId { get; set; }
}
public class GetAllOrdersByPayoutIdQueryHandler : RequestHandlerBase<GetAllOrdersByPayoutIdQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetAllOrdersByPayoutIdQueryHandler(IWalletRepository walletRepository, IServiceProvider serviceProvider, ILogger<GetAllOrdersByPayoutIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrdersByPayoutIdQuery request, CancellationToken cancellationToken)
  {

    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var data = await _walletRepository.GetAllOrdersByPayoutId(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientIdStr!, request.PayoutId);

      serviceResult = new ServiceResultDTO(data);
      return serviceResult;

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
