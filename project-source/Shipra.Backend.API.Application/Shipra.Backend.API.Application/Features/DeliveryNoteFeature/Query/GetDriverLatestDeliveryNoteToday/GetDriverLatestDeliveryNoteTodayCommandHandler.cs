using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDriverLatestDeliveryNoteToday;

public class GetDriverLatestDeliveryNoteTodayCommandHandler : RequestHandlerBase<GetDriverLatestDeliveryNoteTodayQuery, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public GetDriverLatestDeliveryNoteTodayCommandHandler(IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<GetDriverLatestDeliveryNoteTodayCommandHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDriverLatestDeliveryNoteTodayQuery request, CancellationToken cancellationToken)
  {
    var notes = await _deliveryNoteRepository.GetDriverLatestDeliveryNoteToday(request.DriverId, _currentUser.ClientIdStr!);

    var result = new BaseResponseDto();

    if (notes != null && notes.Any())
    {
      var groupedNotes = notes
          .GroupBy(n => new { n.NoteNo, n.CreatedOn, n.TotalPendingCount })
          .Select(g => new
          {
            NoteNo = g.Key.NoteNo,
            CreatedOn = g.Key.CreatedOn,
            TotalPendingCount = g.Key.TotalPendingCount,
            Orders = g.Where(r => r.OrderNo != null).Select(r => new
            {
              OrderNo = r.OrderNo,
              TrackingNo = r.TrackingNo,
              CustomerName = r.CustomerName,
              DeliveryAddress = r.DeliveryAddress,
              OrderDate = r.OrderDate
            }).ToList()
          }).ToList();

      result.Data = groupedNotes;
      result.Message = "Delivery notes found.";
    }
    else
    {
      result.Data = null;
      result.Message = "No delivery note found for today.";
    }

    return new ServiceResultDTO(result);
  }
}
