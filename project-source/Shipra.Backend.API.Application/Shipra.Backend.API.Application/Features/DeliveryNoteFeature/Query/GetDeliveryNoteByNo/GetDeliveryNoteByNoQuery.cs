using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DeliveryNoteUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteByNo;
public class GetDeliveryNoteByNoQuery : IRequest<ServiceResultDTO>
{
  public string? NoteNo { get; set; }
}
public class GetDeliveryNoteByNoQueryHandler : RequestHandlerBase<GetDeliveryNoteByNoQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public GetDeliveryNoteByNoQueryHandler(IDeliveryNoteRepository deliveryNoteRepository,IServiceProvider serviceProvider, ILogger<GetDeliveryNoteByNoQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDeliveryNoteByNoQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    { 
      var deliveryNote = await _deliveryNoteRepository.GetDeliveryNoteByNoteNo(request.NoteNo,_currentUser.ClientIdStr);
       
      if (deliveryNote is not null)
      {
        var mapper = _mapper.Map<DeliveryNoteResponseModel>(deliveryNote);
         serviceResult = new ServiceResultDTO(mapper);
      }
      else
      {
        throw new EntityNotFoundException("DeliveryNote ", request!.NoteNo!);
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetDeliveryNoteByNoQueryValidator : AbstractValidator<GetDeliveryNoteByNoQuery>
{
  public GetDeliveryNoteByNoQueryValidator()
  {
    RuleFor(x =>x.NoteNo).NotEmpty().NotNull();
  }
}
