using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverAccountBalanceById;
public class GetDriverAccountBalanceByIdQuery : IRequest<ServiceResultDTO>
{
}
