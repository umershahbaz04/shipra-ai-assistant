using FluentValidation;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetMyCarrierShipmentsByReturnReportNo;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetMyCarrierShipmentsByReturnReportId;
public class GetMyCarrierShipmentsByReturnReportIdQueryValidator : AbstractValidator<GetMyCarrierShipmentsByReturnReportIdQuery>
{
  public GetMyCarrierShipmentsByReturnReportIdQueryValidator()
  {
    RuleFor(v => v.CarrierRRId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
