using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetAllPayoutFileByPayoutId;
public class GetAllPayoutFileByPayoutIdQuery : IRequest<ServiceResultDTO>
{
  public string? PayoutId { get; set; }
}
public class GetAllPayoutFileByPayoutIdQueryHandler : RequestHandlerBase<GetAllPayoutFileByPayoutIdQuery, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public GetAllPayoutFileByPayoutIdQueryHandler(IWalletRepository walletRepository,IServiceProvider serviceProvider, ILogger<GetAllPayoutFileByPayoutIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllPayoutFileByPayoutIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<PayoutFile> data = await _walletRepository.GetAllPayoutFiles(new PayoutId(new Guid(request.PayoutId!)),_currentUser.ClientId!); 
      serviceResult = new ServiceResultDTO(data.Select(x => new { x.PayoutFileId?.Value,x.FilePath,request.PayoutId}));
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetAllPayoutFileByPayoutIdQueryValidator : AbstractValidator<GetAllPayoutFileByPayoutIdQuery>
{
  public GetAllPayoutFileByPayoutIdQueryValidator()
  {
    RuleFor(x => x.PayoutId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
