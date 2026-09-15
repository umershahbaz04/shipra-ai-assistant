using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.MergeDeliveryNotes;

public class MergeDeliveryNotesCommandHandler : RequestHandlerBase<MergeDeliveryNotesCommand, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public MergeDeliveryNotesCommandHandler(IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<MergeDeliveryNotesCommandHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(MergeDeliveryNotesCommand request, CancellationToken cancellationToken)
  {
    var serviceResultDTO = new ServiceResultDTO();
    try
    {
      if (request.SourceDeliveryNoteIds == null || request.SourceDeliveryNoteIds.Count < 2 || string.IsNullOrEmpty(request.TargetDeliveryNoteId))
      {
        var response = new BaseResponseDto()
        {
          Data = null,
          Message = "Invalid merge parameters. Select at least 2 delivery notes."
        };
        return new ServiceResultDTO(response, false);
      }

      var success = await _deliveryNoteRepository.MergeDeliveryNotes(request.TargetDeliveryNoteId, request.SourceDeliveryNoteIds, request.CreatedDate, _currentUser.EmployeeId!);
      if (success)
      {
        var response = new BaseResponseDto()
        {
          Data = true,
          Message = "Delivery Notes merged successfully."
        };
        return new ServiceResultDTO(response);
      }
      else
      {
        var response = new BaseResponseDto()
        {
          Data = false,
          Message = "Failed to merge delivery notes. Target note not found."
        };
        return new ServiceResultDTO(response, false);
      }
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      return serviceResultDTO;
    }
  }
}
