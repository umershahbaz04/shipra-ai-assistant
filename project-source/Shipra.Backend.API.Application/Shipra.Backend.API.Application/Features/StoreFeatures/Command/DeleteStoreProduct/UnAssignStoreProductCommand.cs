using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.DeleteStoreProduct;
public class UnAssignStoreProductCommand : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public List<AssignStoreProductCreateModal>? list { get; set; }
}
public class UnAssignStoreProductCommandHandler : RequestHandlerBase<UnAssignStoreProductCommand, ServiceResultDTO>
{
  private readonly IStoreRepository _storeRepository;

  public UnAssignStoreProductCommandHandler(IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<UnAssignStoreProductCommandHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UnAssignStoreProductCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    {
      foreach (var item in request.list!)
      {

        StoreProduct existStore = await _storeRepository.CheckExistProductOnStore(request.StoreId, new ProductId(new Guid(item.ProductId!)));
        if (existStore is not null)
        {
          if (existStore.Active.GetValueOrDefault())
          {
            existStore.DeleteStoreProduct(_currentUser.EmployeeId);
            await _storeRepository.UpdateStoreProduct(existStore);
          }
        } 
      } 
      response = new ServiceResultDTO(new BaseResponseDto { Data = null, Message = "Product un assigned to store successfully." });
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;

    }
  }
}
