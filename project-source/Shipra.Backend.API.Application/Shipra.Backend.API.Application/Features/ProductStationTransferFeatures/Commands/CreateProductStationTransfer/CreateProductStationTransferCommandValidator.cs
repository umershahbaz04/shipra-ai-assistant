using FluentValidation;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;
using Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateProduct;

namespace Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Commands.CreateProductStationTransfer;
public class CreateProductStationTransferCommandValidator : AbstractValidator<CreateProductStationTransferCommand>
{
  public CreateProductStationTransferCommandValidator()
  {

    RuleFor(v => v.TrackingNo).NotNull().NotEmpty();
    RuleFor(v => v.TransferStatusId).NotNull().NotEmpty();
    RuleFor(v => v.ExpectedArrivalTime).NotNull().NotEmpty();
    RuleFor(v => v.OriginProductStationId).NotNull().NotEmpty();
    RuleFor(v => v.DestinationProductStationId).NotNull().NotEmpty();
    RuleFor(x => x.TransferProducts).Must(x => x != null).WithMessage("Transfer Product list must contain at least one item.");

    RuleForEach(model => model.TransferProducts).SetValidator(model => new TransferProductsValidator()); 
  }
}
public class TransferProductsValidator : AbstractValidator<TransferProductRequestModel>
{
  public TransferProductsValidator()
  {
    RuleFor(v => v.ProductId).NotNull().NotEmpty();
    RuleFor(v => v.Accepted).NotNull().NotEmpty();
    RuleFor(x => x.ProductId)
            .Must(GuidHelper.Validator).WithMessage("Supplied Id is not correct");
  }
}
