using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.TotalProcessFeature.Query.GetAllPaymentLinkStatusLookup;
public class GetAllPaymentLinkStatusLookupQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllPaymentLinkStatusLookupQueryHandler : RequestHandlerBase<GetAllPaymentLinkStatusLookupQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetAllPaymentLinkStatusLookupQueryHandler(IWalletRepository walletRepository,IServiceProvider serviceProvider, ILogger<GetAllPaymentLinkStatusLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPaymentLinkStatusLookupQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    {
      var allData = await _walletRepository.GetAllPaymentLinkStatusLookup();
      var obj = PaymentLinkStatusLookup.AddDefault();
      allData?.Add(obj);
      var data = allData!.OrderBy(x => x.PaymentLinkStatusId).ToList();
      response = new ServiceResultDTO(data);
      response.CreateSuccessResponse();
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
