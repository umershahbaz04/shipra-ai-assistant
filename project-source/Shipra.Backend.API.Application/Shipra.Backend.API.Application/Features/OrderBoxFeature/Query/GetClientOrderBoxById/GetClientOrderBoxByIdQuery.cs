using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderBoxFeature.Query.GetClientOrderBoxById;
public class GetClientOrderBoxByIdQuery : IRequest<ServiceResultDTO> 
{
  public int ClientOrderBoxId { get;  set; }

}
public class GetClientOrderBoxByIdQueryHandler : RequestHandlerBase<GetClientOrderBoxByIdQuery, ServiceResultDTO>
{
  private readonly IOrderBoxRepository _orderBoxRepository;

  public GetClientOrderBoxByIdQueryHandler(IOrderBoxRepository orderBoxRepository,IServiceProvider serviceProvider, ILogger<GetClientOrderBoxByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderBoxRepository = orderBoxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientOrderBoxByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var clientOrderBox = await _orderBoxRepository.GetClientOrderBoxById(request.ClientOrderBoxId,_currentUser.ClientId!);
      if (clientOrderBox is null)
      {
        throw new EntityNotFoundException("ClientOrderBox ", request.ClientOrderBoxId!);
      } 
      var mapData = _mapper.Map<ClientOrderBoxResponseModel>(clientOrderBox);  

      serviceResult = new ServiceResultDTO(mapData);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetClientOrderBoxByIdQueryValidator : AbstractValidator<GetClientOrderBoxByIdQuery>
{
  public GetClientOrderBoxByIdQueryValidator()
  {
    RuleFor(x => x.ClientOrderBoxId).NotEmpty().NotNull(); 
  } 

}
