using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.DeleteStoreCommand;
public class DeleteStoreCommandHandler : RequestHandlerBase<DeleteStoreCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IStoreRepository _storeRepository;
  public DeleteStoreCommandHandler(IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<DeleteStoreCommandHandler> logger) : base(serviceProvider, logger)
  {
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(DeleteStoreCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    try
    {
      var store = await _storeRepository.GetStoreById(request.StoreId, _currentUser.ClientId!);
      if (store == null)
      {
        throw new EntityNotFoundException("Store ", request.StoreId);
      }
      store.DisableStore(_currentUser.EmployeeId!);
      response.IsSuccess = await _storeRepository.DisableStore(store);
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }

  }
}

