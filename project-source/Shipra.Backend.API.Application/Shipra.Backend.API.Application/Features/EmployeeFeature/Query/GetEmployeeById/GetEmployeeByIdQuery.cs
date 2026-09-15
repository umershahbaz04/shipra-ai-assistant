using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.EmployeeUseCase;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetEmployeeById;
public class GetEmployeeByIdQuery : IRequest<ServiceResultDTO>
{
  public string? EmployeeId { get; set; }
}
public class GetEmployeeByIdQueryHandler : RequestHandlerBase<GetEmployeeByIdQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly IEmployeeRepository _employeeRepository;

  public GetEmployeeByIdQueryHandler(ISaleChannelConfigRepository saleChannelConfigRepository, IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<GetEmployeeByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      EmployeeId employeeId = new EmployeeId(new Guid(request.EmployeeId!));
      var oEmployee = await _employeeRepository.GetEmployeeById(employeeId!, _currentUser.ClientId!);
      if (oEmployee is null)
      {
        throw new EntityNotFoundException("Employee", request.EmployeeId!);
      }
      var oEmployeeAddress = await _employeeRepository.GetEmployeeAddressById(oEmployee.EmployeeId);
      if (oEmployeeAddress is null)
      {
        throw new EntityNotFoundException("Employee Address", request.EmployeeId!);
      }
      int? storeId = null;
      if (oEmployee is not null)
      {
        if (oEmployee.SaleChannelConfigId is not null)
        {
          var oSc = await _saleChannelConfigRepository.GetSaleChannelConfigById(oEmployee.SaleChannelConfigId.GetValueOrDefault(), _currentUser.ClientId!);
          if (oSc is not null)
          { 
            storeId = oSc.StoreId;
          }

        }
      }
      var employeeMap = _mapper.Map<EmployeResponseModel>(oEmployee);
      var oAddressMap = _mapper.Map<AddressResponseDTO>(oEmployeeAddress);
      employeeMap.Address = oAddressMap;
      employeeMap.StoreId = storeId;
      serviceResult = new ServiceResultDTO(employeeMap);

      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {

      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
  public GetEmployeeByIdQueryValidator()
  {
    RuleFor(x => x.EmployeeId).NotNull().NotEmpty();
  }
}
