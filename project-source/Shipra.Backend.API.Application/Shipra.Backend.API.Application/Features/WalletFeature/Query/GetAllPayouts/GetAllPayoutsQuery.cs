using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllPayouts;
public class GetAllPayoutsQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int? PayoutStatusId { get; set; }
}
public class GetAllPayoutsQueryHandler : RequestHandlerBase<GetAllPayoutsQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetAllPayoutsQueryHandler(IWalletRepository walletRepository,IServiceProvider serviceProvider, ILogger<GetAllPayoutsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPayoutsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic data = await _walletRepository.GetAllPayouts(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.PayoutStatusId);
       
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
