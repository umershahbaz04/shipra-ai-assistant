using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.EmployeeUseCase;
using Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.CreateStore;
public class CreateStoreCommand : CreateStoreRequestModel, IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{ 

}
