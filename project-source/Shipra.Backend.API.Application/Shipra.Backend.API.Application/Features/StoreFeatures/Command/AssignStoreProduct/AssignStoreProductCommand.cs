using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.AssignStoreProduct;
public class AssignStoreProductCommand : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public List<AssignStoreProductCreateModal>? list { get; set; }
}
public class AssignStoreProductCommandHandler : RequestHandlerBase<AssignStoreProductCommand, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;

  public AssignStoreProductCommandHandler(IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<AssignStoreProductCommandHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AssignStoreProductCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    {
      foreach (var item in request.list!)
      {

        StoreProduct existStore = await _storeRepository.CheckExistProductOnStore(request.StoreId, new ProductId(new Guid(item.ProductId!)));
        if (existStore is null)
        {
          StoreProduct storeProduct = StoreProduct.CreateStoreProduct(request.StoreId, new ProductId(new Guid(item.ProductId!)), _currentUser.EmployeeId);
          await _storeRepository.CreateStoreProduct(storeProduct);
        }
        else
        {
          if (item.Active.GetValueOrDefault())
          {
            existStore.Activate(_currentUser.EmployeeId); 
          }
          else
          {
            existStore.DeleteStoreProduct(_currentUser.EmployeeId); 
          }
          await _storeRepository.UpdateStoreProduct(existStore);
        }
      }
      response = new ServiceResultDTO(new BaseResponseDto { Data = null, Message = "Product assigned to store successfully." });
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;

    }
  }
}
public class AssignStoreProductCommandValidator : AbstractValidator<AssignStoreProductCommand>
{
  public AssignStoreProductCommandValidator()
  {
    RuleFor(x => x.StoreId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.list).Must(x => x != null).WithMessage("list must contain at least one item.");

    RuleForEach(x => x.list).SetValidator(x => new AssignStoreProductCommandProductValidator());
  }
}
public class AssignStoreProductCommandProductValidator : AbstractValidator<AssignStoreProductCreateModal>
{
  public AssignStoreProductCommandProductValidator()
  {
    RuleFor(v => v.ProductId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
