using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.GenerateEncryptedKeyAgainstStoreAndStation;
public class GenerateEncryptedKeyAgainstStoreAndStationCommand : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public int StationId { get; set; }
}
public class GenerateEncryptedKeyAgainstStoreAndStationCommandHandler : RequestHandlerBase<GenerateEncryptedKeyAgainstStoreAndStationCommand, ServiceResultDTO>
{
  private readonly IProductStationRepository _productStationRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IStoreRepository _storeRepository;

  public GenerateEncryptedKeyAgainstStoreAndStationCommandHandler(IProductStationRepository productStationRepository,IClientRepository clientRepository, IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<GenerateEncryptedKeyAgainstStoreAndStationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productStationRepository = productStationRepository;
    _clientRepository = clientRepository;
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GenerateEncryptedKeyAgainstStoreAndStationCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client == null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!);
      }
      var target = await _storeRepository.GetStoreById(request.StoreId, _currentUser.ClientId!);
      if (target == null)
      {
        throw new EntityNotFoundException("Store ", request.StoreId);
      }
      
      var productStations = await _productStationRepository.GetAllProductStations(_currentUser.ClientId!);
      var ps = productStations!.FirstOrDefault(x => x.ProductStationId == request.StationId);
      if (ps == null)
      {
        throw new EntityNotFoundException("ProductStation ", request.StationId);
      }
      #region encyptedkey
      string? json = UtilityHelper.ToBase64(new
      {
        storeId = target.StoreId,
        storeName = target.StoreName,
        stationId = request.StationId
      });
      string encryptedKey = client.EncryptedKey + ":" + json;
      #endregion


      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = encryptedKey, Message = "Update successfully" });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GenerateEncryptedKeyAgainstStoreAndStationCommandValidator : AbstractValidator<GenerateEncryptedKeyAgainstStoreAndStationCommand>
{
  public GenerateEncryptedKeyAgainstStoreAndStationCommandValidator()
  {
    RuleFor(x => x.StoreId).NotEmpty().NotNull().GreaterThan(0);
    RuleFor(x => x.StationId).NotEmpty().NotNull().GreaterThan(0);
  } 
}
