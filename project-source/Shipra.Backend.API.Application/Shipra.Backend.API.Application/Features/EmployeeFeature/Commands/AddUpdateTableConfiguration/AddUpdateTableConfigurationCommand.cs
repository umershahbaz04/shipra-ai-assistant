using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.AddUpdateTableConfiguration;
public class AddUpdateTableConfigurationCommand : IRequest<ServiceResultDTO>
{
  public string? TableName { get; set; }
  public Dictionary<string, bool>? dataModel { get; set; }
}
public class AddUpdateTableConfigurationCommandHandler : RequestHandlerBase<AddUpdateTableConfigurationCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;

  public AddUpdateTableConfigurationCommandHandler(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<AddUpdateTableConfigurationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AddUpdateTableConfigurationCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var config = JsonConvert.SerializeObject(new
      {
        tableName = request.TableName,
        columnVisibilityModel = request.dataModel
      }, Formatting.Indented);


      var oList = await _employeeRepository.GetAllEmployeeColumnConfiguration(_currentUser.ClientId);

      var oEmployeeColumnConfiguration = oList.FirstOrDefault(x => x.TableName == request.TableName && x.EmployeeId == _currentUser.EmployeeId);
      if (oEmployeeColumnConfiguration is null)
      {
        oEmployeeColumnConfiguration = EmployeeColumnConfiguration.Create(_currentUser.EmployeeId!, _currentUser.ClientId!, request.TableName, config);
        serviceResult.IsSuccess = await _employeeRepository.CreateEmployeeColumnConfiguration(oEmployeeColumnConfiguration);
      }
      else
      {
        oEmployeeColumnConfiguration.Update(config);
        serviceResult.IsSuccess = await _employeeRepository.UpdateEmployeeColumnConfiguration(oEmployeeColumnConfiguration);
      }
      if (serviceResult.IsSuccess)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oEmployeeColumnConfiguration.ColumnConfigurationId, Message = "Action perform successfully" });
      }
      else
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oEmployeeColumnConfiguration.ColumnConfigurationId, Message = "Something went wrong" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
public class AddUpdateTableConfigurationCommandValidator : AbstractValidator<AddUpdateTableConfigurationCommand>
{
  public string? TableName { get; set; }
  public Dictionary<string, bool>? dataModel { get; set; }
}
